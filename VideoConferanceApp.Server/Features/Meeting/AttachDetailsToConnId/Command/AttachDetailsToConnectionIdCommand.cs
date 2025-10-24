using MediatR;
using VideoConferanceApp.Shared.Meeting.Requests;
using VideoConferanceApp.Shared.Meeting.Responses;

namespace VideoConferanceApp.Server.Features.Meeting.AttachDetailsToConnId.Command;

public record AttachDetailsToConnectionIdCommand(AttachDetailsToConnectionIdRequest DetailsToConnectionId) :
    IRequest<AttachDetailsToConnectionIdResponse>;