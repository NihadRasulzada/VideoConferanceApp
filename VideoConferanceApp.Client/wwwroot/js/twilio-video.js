
let localAudioTrack = null;
let localTracks = [];
let isAudioMuted = false;
let activeRoom = null;
async function connectToRoom(token, roomName, containerId) {

    // Connect to the Twilio Video Room
    localTracks = await Twilio.Video.createLocalTracks({ audio: true, video: true });

    // Store video & audio track reference
    localAudioTrack = localTracks.find(track => track.kind === 'audio');
    const videoTrack = localTracks.find(track => track.kind === 'video');
    if (videoTrack) {
        addTrackToDOM(videoTrack, containerId);
    }
    activeRoom = await Twilio.Video.connect(token, {
        name: roomName,
        tracks: localTracks
    });

    // Attach remote participants
    activeRoom.participants.forEach(participant => {
        attachParticipantTracks(participant, containerId);
    });

    // Listen for new participants and attach them
    activeRoom.on('participantConnected', participant => {
        console.log(`Participant Connected: ${participant.identity}`);
        attachParticipantTracks(participant, containerId);
    });

    // Listen for participant disconnection and detach them
    activeRoom.on('participantDisconnected', participant => {
        console.log(`Participant Disconnected: ${participant.identity}`);
        detachParticipantTracks(participant);
    });

    // fire when you disconnect
    activeRoom.on('disconnected', () => {
        console.log("You have left the room");
        detachAllTracks(containerId);
    });
}

function attachParticipantTracks(participant, containerId) {
    participant.tracks.forEach(publication => {
        if (publication.isSubscribed) {
            addTrackToDOM(publication.track, containerId);
        }
    });

    participant.on('trackSubscribed', track => {
        addTrackToDOM(track, containerId);
    });

    participant.on('trackUnsubscribed', track => {
        track.detach().forEach(element => element.remove());
    });
}

function detachParticipantTracks(participant) {
    participant.tracks.forEach(publication => {
        if (publication.track) {
            publication.track.detach().forEach(element => element.remove());
        }
    });
}

function addTrackToDOM(track, containerId) {
    const container = document.getElementById(containerId);

    if (!container) {
        console.error(`Container with ID ${containerId} not found!`);
        return;
    }

    const trackElement = track.attach();
    container.appendChild(trackElement);
    console.log('Appended track element:', trackElement);
}

// Toggle Audio (Mute/Unmute)
function toggleAudio() {
    if (localAudioTrack) {
        isAudioMuted = !isAudioMuted;
        localAudioTrack.enable(!isAudioMuted); // Toggle audio state
        console.log(`Audio ${isAudioMuted ? 'Muted' : 'Unmuted'}`);
    }
}

function leaveRoom(containerId) {
    if (activeRoom && activeRoom.localParticipant) {
        // Stop and detach all local tracks (audio & video)
        activeRoom.localParticipant.tracks.forEach(publication => {
            if (publication.track) {
                publication.track.stop(); // Stop capturing from device
                publication.track.detach().forEach(element => element.remove());
            }
        });
    }

    // Also, stop tracks stored in localTracks in case they weren't cleaned up
    if (localTracks && localTracks.length) {
        localTracks.forEach(track => {
            track.stop();
            track.detach().forEach(element => element.remove());
        });
        localTracks = []; // Clear out our stored tracks
    }

    // Disconnect from the room
    if (activeRoom) {
        activeRoom.disconnect();
        activeRoom = null;
    }

    // Clear UI container of any remaining track elements
    detachAllTracks(containerId);
    console.log("Left the meeting and stopped local camera and audio");
}

// Remove all tracks from UI
function detachAllTracks(containerId) {
    document.getElementById(containerId).innerHTML = "";
}

window.twilioVideo = {
    connectToRoom,
    toggleAudio,
    leaveRoom
}