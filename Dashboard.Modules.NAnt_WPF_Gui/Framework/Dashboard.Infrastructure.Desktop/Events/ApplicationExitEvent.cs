using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Events;

namespace Techno_Fly.Tools.Dashboard.Events
{
    public class ApplicationExitEvent : CompositePresentationEvent<CancelableEventArgs<object>>
    {
        /* Intentionally left blank. */
    }
}