﻿using ExportPro.Common.Shared.Library;
using MediatR;

namespace ExportPro.Common.Shared.Mediator;

public interface ICommand : IRequest<BaseResponse> { }
