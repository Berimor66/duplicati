﻿using System;
using System.Collections.Generic;

namespace Duplicati.Server.Serialization
{
    public interface IServerStatus
    {
        RunnerState ActiveBackupState { get; }
        long ActiveScheduleId { get; }
        LiveControlState ProgramState { get; }
        IProgressEventData RunningBackupStatus { get; }
        System.Collections.Generic.IList<long> SchedulerQueueIds { get; }
        bool HasWarning { get; }
        bool HasError { get; }
        SuggestedStatusIcon SuggestedStatusIcon { get; }
    }
}
