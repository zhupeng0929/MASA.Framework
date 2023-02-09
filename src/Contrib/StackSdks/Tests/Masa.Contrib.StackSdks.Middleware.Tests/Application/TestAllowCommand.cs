// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Middleware.Tests.Application;

[AllowedEvent]
public record TestAllowCommand : Command
{
    public int Count { get; set; }
}