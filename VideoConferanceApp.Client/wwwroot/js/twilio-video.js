// Custom WebRTC implementation replacing Twilio Video SDK
// Signaling is done via SignalR (relay through MeetingHub)
// DotNet reference (dotNetRef) is used to call C# [JSInvokable] methods

let dotNetRef = null;
let peerConnections = {};   // peerId (SignalR connection ID) → RTCPeerConnection
let localStream = null;
let activeContainerId = null;

const ICE_SERVERS = [
    { urls: 'stun:stun.l.google.com:19302' },
    { urls: 'stun:stun1.l.google.com:19302' }
];

// Called by Blazor before JoinMeeting to wire up the signaling callbacks
function setupSignaling(ref) {
    dotNetRef = ref;
}

// Called by Blazor (twilioService.JoinMeeting) to start local media
async function connectToRoom(token, roomName, containerId) {
    activeContainerId = containerId;
    try {
        localStream = await navigator.mediaDevices.getUserMedia({ video: true, audio: true });
        addVideoToDOM(localStream, containerId, 'local-video', true);
        console.log('[WebRTC] Local media started');
    } catch (err) {
        console.error('[WebRTC] Failed to get local media:', err);
    }
}

// Called by Blazor when hub sends "ReceiveExistingParticipants"
// New joiner creates offers to all existing participants
async function receiveExistingParticipants(participantIds) {
    if (!participantIds || participantIds.length === 0) return;
    console.log('[WebRTC] Existing participants:', participantIds);
    for (const peerId of participantIds) {
        try {
            const pc = createPeerConnection(peerId);
            const offer = await pc.createOffer();
            await pc.setLocalDescription(offer);
            await dotNetRef.invokeMethodAsync('SendOffer', peerId, JSON.stringify(offer));
            console.log('[WebRTC] Offer sent to', peerId);
        } catch (err) {
            console.error('[WebRTC] Error creating offer for', peerId, err);
        }
    }
}

// Called by Blazor when hub sends "ReceiveOffer"
async function handleOffer(fromId, sdpJson) {
    console.log('[WebRTC] Received offer from', fromId);
    try {
        const pc = createPeerConnection(fromId);
        await pc.setRemoteDescription(new RTCSessionDescription(JSON.parse(sdpJson)));
        const answer = await pc.createAnswer();
        await pc.setLocalDescription(answer);
        await dotNetRef.invokeMethodAsync('SendAnswer', fromId, JSON.stringify(answer));
        console.log('[WebRTC] Answer sent to', fromId);
    } catch (err) {
        console.error('[WebRTC] Error handling offer from', fromId, err);
    }
}

// Called by Blazor when hub sends "ReceiveAnswer"
async function handleAnswer(fromId, sdpJson) {
    console.log('[WebRTC] Received answer from', fromId);
    const pc = peerConnections[fromId];
    if (pc) {
        try {
            await pc.setRemoteDescription(new RTCSessionDescription(JSON.parse(sdpJson)));
        } catch (err) {
            console.error('[WebRTC] Error handling answer from', fromId, err);
        }
    }
}

// Called by Blazor when hub sends "ReceiveIceCandidate"
async function handleIceCandidate(fromId, candidateJson) {
    const pc = peerConnections[fromId];
    if (pc && candidateJson) {
        try {
            await pc.addIceCandidate(new RTCIceCandidate(JSON.parse(candidateJson)));
        } catch (err) {
            console.error('[WebRTC] Error adding ICE candidate from', fromId, err);
        }
    }
}

function createPeerConnection(peerId) {
    if (peerConnections[peerId]) return peerConnections[peerId];

    const pc = new RTCPeerConnection({ iceServers: ICE_SERVERS });

    // Add local tracks to the peer connection
    if (localStream) {
        localStream.getTracks().forEach(track => pc.addTrack(track, localStream));
    }

    // Send ICE candidates to the remote peer via SignalR
    pc.onicecandidate = event => {
        if (event.candidate && dotNetRef) {
            dotNetRef.invokeMethodAsync(
                'SendIceCandidate', peerId, JSON.stringify(event.candidate));
        }
    };

    // When remote track arrives, render it in the container
    pc.ontrack = event => {
        if (event.streams && event.streams[0]) {
            addVideoToDOM(event.streams[0], activeContainerId, `video-${peerId}`, false);
            console.log('[WebRTC] Remote track received from', peerId);
        }
    };

    pc.onconnectionstatechange = () => {
        console.log('[WebRTC] Connection state with', peerId, ':', pc.connectionState);
        if (pc.connectionState === 'disconnected' || pc.connectionState === 'failed') {
            removeVideoFromDOM(`video-${peerId}`);
        }
    };

    peerConnections[peerId] = pc;
    return pc;
}

function addVideoToDOM(stream, containerId, id, muted) {
    const container = document.getElementById(containerId);
    if (!container) {
        console.error('[WebRTC] Container not found:', containerId);
        return;
    }
    let video = document.getElementById(id);
    if (!video) {
        video = document.createElement('video');
        video.id = id;
        video.autoplay = true;
        video.muted = muted;
        video.playsInline = true;
        container.appendChild(video);
    }
    video.srcObject = stream;
}

function removeVideoFromDOM(id) {
    const el = document.getElementById(id);
    if (el) el.remove();
}

// Toggle microphone mute/unmute
function toggleAudio() {
    if (localStream) {
        localStream.getAudioTracks().forEach(track => {
            track.enabled = !track.enabled;
            console.log('[WebRTC] Audio', track.enabled ? 'unmuted' : 'muted');
        });
    }
}

// Leave the meeting: close all peer connections, stop local media, clear UI
function leaveRoom(containerId) {
    Object.entries(peerConnections).forEach(([peerId, pc]) => {
        pc.close();
        removeVideoFromDOM(`video-${peerId}`);
    });
    peerConnections = {};

    if (localStream) {
        localStream.getTracks().forEach(track => track.stop());
        localStream = null;
    }

    const container = document.getElementById(containerId);
    if (container) container.innerHTML = '';

    dotNetRef = null;
    activeContainerId = null;
    console.log('[WebRTC] Left the meeting');
}

window.twilioVideo = {
    setupSignaling,
    connectToRoom,
    receiveExistingParticipants,
    handleOffer,
    handleAnswer,
    handleIceCandidate,
    toggleAudio,
    leaveRoom
};
