using System;
using System.Collections.ObjectModel;
using System.ServiceModel.Configuration;

namespace WcfServiceTest
{
    public class SessionTokenBehaviorExtension : BehaviorExtensionElement
    {
        protected override object CreateBehavior()
        {
            return new SessionTokenBehavior();
        }

        public override Type BehaviorType
        {
            get { return typeof(SessionTokenBehavior); }
        }
    }
}