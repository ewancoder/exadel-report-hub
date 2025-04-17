﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExportPro.StorageService.CQRS.Handlers.Client;
using ExportPro.StorageService.DataAccess.Interfaces;
using FluentValidation;

namespace ExportPro.StorageService.Validations.Validations.Client;

public sealed class CreateClientCommandValidator : AbstractValidator<CreateClientCommand>
{

    public CreateClientCommandValidator(IClientRepository clientRepository)
    {
        RuleFor(x => x.Clientdto.Name)
            .SetValidator(new ClientNameValidator(clientRepository));
    }
}
