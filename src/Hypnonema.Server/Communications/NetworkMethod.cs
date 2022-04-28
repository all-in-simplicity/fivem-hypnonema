namespace Hypnonema.Server.Communications
{
    using System;

    using CitizenFX.Core;

    using Newtonsoft.Json;

    public abstract class NetworkMethod : IDisposable
    {
        protected NetworkMethod(string eventName, Delegate callback = null)
        {
            this.EventName = eventName;
            this.Callback = callback;
            this.RegisteredCallback = this.GetRegisterCallback();
            BaseServer.Self.AddEvent("hyp:C2S:" + this.EventName, this.RegisteredCallback);
        }

        ~NetworkMethod()
        {
            this.Dispose();
        }

        public string EventName { get; }

        protected Delegate Callback { get; private set; }

        protected Delegate RegisteredCallback { get; }

        /// <inheritdoc />
        public void Dispose()
        {
            if (this.Callback != null)
            {
                BaseServer.Self.RemoveEvent("hyp:S2C:" + this.EventName, this.RegisteredCallback);
                this.Callback = null;
            }

            GC.SuppressFinalize(this);
        }

        public void Invoke(Player target)
        {
            this.InvokeInternal(target);
        }

        public void InvokeNoArgs(Player target)
        {
            this.InvokeInternal(target);
        }

        // Simple wrappers around JsonConvert to handle null values being passed in.
        // This happens when you trigger "server" events that expect a reply to the same NetworkMethod handler.
        protected static T DeserializeObject<T>(string text)
        {
            return text == null ? default : JsonConvert.DeserializeObject<T>(text);
        }

        protected static string SerializeObject<T>(T o)
        {
            return o == null ? null : JsonConvert.SerializeObject(o);
        }

        protected virtual Delegate GetRegisterCallback()
        {
            return this.Callback;
        }

        protected void InvokeInternal(Player target, params object[] args)
        {
            if (target != null)
                BaseScript.TriggerClientEvent(target, "hyp:S2C:" + this.EventName, args);
            else
                BaseScript.TriggerClientEvent("hyp:S2C:" + this.EventName, args);
        }
    }

    public class NetworkMethod<T1> : NetworkMethod
    {
        public NetworkMethod(string eventName, Action<Player, T1> callback = null)
            : base(eventName, callback)
        {
        }

        public void Invoke(Player target, T1 value1)
        {
            this.InvokeInternal(target, SerializeObject(value1));
        }

        protected override Delegate GetRegisterCallback()
        {
            return new Action<Player, object>(this.SerializedCallback);
        }

        private void SerializedCallback([FromSource] Player player, object val1)
        {
            this.Callback.DynamicInvoke(player, DeserializeObject<T1>((string) val1));
        }
    }

    public class NetworkMethod<T1, T2> : NetworkMethod
    {
        public NetworkMethod(string eventName, Action<Player, T1, T2> callback = null)
            : base(eventName, callback)
        {
        }

        public void Invoke(Player target, T1 value1, T2 value2)
        {
            this.InvokeInternal(target, SerializeObject(value1), SerializeObject(value2));
        }

        protected override Delegate GetRegisterCallback()
        {
            return new Action<Player, object, object>(this.SerializedCallback);
        }

        private void SerializedCallback([FromSource] Player player, object val1, object val2)
        {
            this.Callback.DynamicInvoke(
                player,
                DeserializeObject<T1>((string) val1),
                DeserializeObject<T2>((string) val2));
        }
    }

    public class NetworkMethod<T1, T2, T3> : NetworkMethod
    {
        public NetworkMethod(string eventName, Action<Player, T1, T2, T3> callback = null)
            : base(eventName, callback)
        {
        }

        public void Invoke(Player target, T1 value1, T2 value2, T3 value3)
        {
            this.InvokeInternal(target, SerializeObject(value1), SerializeObject(value2), SerializeObject(value3));
        }

        protected override Delegate GetRegisterCallback()
        {
            return new Action<Player, object, object, object>(this.SerializedCallback);
        }

        private void SerializedCallback([FromSource] Player player, object val1, object val2, object val3)
        {
            this.Callback.DynamicInvoke(
                player,
                DeserializeObject<T1>((string) val1),
                DeserializeObject<T2>((string) val2),
                DeserializeObject<T3>((string) val3));
        }
    }

    public class NetworkMethod<T1, T2, T3, T4> : NetworkMethod
    {
        public NetworkMethod(string eventName, Action<Player, T1, T2, T3, T4> callback = null)
            : base(eventName, callback)
        {
        }

        public void Invoke(Player target, T1 value1, T2 value2, T3 value3, T4 value4)
        {
            this.InvokeInternal(
                target,
                SerializeObject(value1),
                SerializeObject(value2),
                SerializeObject(value3),
                SerializeObject(value4));
        }

        protected override Delegate GetRegisterCallback()
        {
            return new Action<Player, object, object, object, object>(this.SerializedCallback);
        }

        private void SerializedCallback([FromSource] Player player, object val1, object val2, object val3, object val4)
        {
            this.Callback.DynamicInvoke(
                player,
                DeserializeObject<T1>((string) val1),
                DeserializeObject<T2>((string) val2),
                DeserializeObject<T3>((string) val3),
                DeserializeObject<T4>((string) val4));
        }
    }

    public class NetworkMethod<T1, T2, T3, T4, T5> : NetworkMethod
    {
        public NetworkMethod(string eventName, Action<Player, T1, T2, T3, T4, T5> callback = null)
            : base(eventName, callback)
        {
        }

        public void Invoke(Player target, T1 value1, T2 value2, T3 value3, T4 value4, T5 value5)
        {
            this.InvokeInternal(
                target,
                SerializeObject(value1),
                SerializeObject(value2),
                SerializeObject(value3),
                SerializeObject(value4),
                SerializeObject(value5));
        }

        protected override Delegate GetRegisterCallback()
        {
            return new Action<Player, object, object, object, object, object>(this.SerializedCallback);
        }

        private void SerializedCallback(
            [FromSource] Player player,
            object val1,
            object val2,
            object val3,
            object val4,
            object val5)
        {
            this.Callback.DynamicInvoke(
                player,
                DeserializeObject<T1>((string) val1),
                DeserializeObject<T2>((string) val2),
                DeserializeObject<T3>((string) val3),
                DeserializeObject<T4>((string) val4),
                DeserializeObject<T5>((string) val5));
        }
    }

    public class NetworkMethod<T1, T2, T3, T4, T5, T6> : NetworkMethod
    {
        public NetworkMethod(string eventName, Action<Player, T1, T2, T3, T4, T5, T6> callback = null)
            : base(eventName, callback)
        {
        }

        public void Invoke(Player target, T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6)
        {
            this.InvokeInternal(
                target,
                SerializeObject(value1),
                SerializeObject(value2),
                SerializeObject(value3),
                SerializeObject(value4),
                SerializeObject(value5),
                SerializeObject(value6));
        }

        protected override Delegate GetRegisterCallback()
        {
            return new Action<Player, object, object, object, object, object, object>(this.SerializedCallback);
        }

        private void SerializedCallback(
            [FromSource] Player player,
            object val1,
            object val2,
            object val3,
            object val4,
            object val5,
            object val6)
        {
            this.Callback.DynamicInvoke(
                player,
                DeserializeObject<T1>((string) val1),
                DeserializeObject<T2>((string) val2),
                DeserializeObject<T3>((string) val3),
                DeserializeObject<T4>((string) val4),
                DeserializeObject<T5>((string) val5),
                DeserializeObject<T6>((string) val6));
        }
    }

    public class NetworkMethod<T1, T2, T3, T4, T5, T6, T7> : NetworkMethod
    {
        public NetworkMethod(string eventName, Action<Player, T1, T2, T3, T4, T5, T6, T7> callback = null)
            : base(eventName, callback)
        {
        }

        public void Invoke(Player target, T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7)
        {
            this.InvokeInternal(
                target,
                SerializeObject(value1),
                SerializeObject(value2),
                SerializeObject(value3),
                SerializeObject(value4),
                SerializeObject(value5),
                SerializeObject(value6),
                SerializeObject(value7));
        }

        protected override Delegate GetRegisterCallback()
        {
            return new Action<Player, object, object, object, object, object, object, object>(this.SerializedCallback);
        }

        private void SerializedCallback(
            [FromSource] Player player,
            object val1,
            object val2,
            object val3,
            object val4,
            object val5,
            object val6,
            object val7)
        {
            this.Callback.DynamicInvoke(
                player,
                DeserializeObject<T1>((string) val1),
                DeserializeObject<T2>((string) val2),
                DeserializeObject<T3>((string) val3),
                DeserializeObject<T4>((string) val4),
                DeserializeObject<T5>((string) val5),
                DeserializeObject<T6>((string) val6),
                DeserializeObject<T7>((string) val7));
        }
    }

    public class NetworkMethod<T1, T2, T3, T4, T5, T6, T7, T8> : NetworkMethod
    {
        public NetworkMethod(string eventName, Action<Player, T1, T2, T3, T4, T5, T6, T7, T8> callback = null)
            : base(eventName, callback)
        {
        }

        public void Invoke(
            Player target,
            T1 value1,
            T2 value2,
            T3 value3,
            T4 value4,
            T5 value5,
            T6 value6,
            T7 value7,
            T8 value8)
        {
            this.InvokeInternal(
                target,
                SerializeObject(value1),
                SerializeObject(value2),
                SerializeObject(value3),
                SerializeObject(value4),
                SerializeObject(value5),
                SerializeObject(value6),
                SerializeObject(value7),
                SerializeObject(value8));
        }

        protected override Delegate GetRegisterCallback()
        {
            return new Action<Player, object, object, object, object, object, object, object, object>(
                this.SerializedCallback);
        }

        private void SerializedCallback(
            [FromSource] Player player,
            object val1,
            object val2,
            object val3,
            object val4,
            object val5,
            object val6,
            object val7,
            object val8)
        {
            this.Callback.DynamicInvoke(
                player,
                DeserializeObject<T1>((string) val1),
                DeserializeObject<T2>((string) val2),
                DeserializeObject<T3>((string) val3),
                DeserializeObject<T4>((string) val4),
                DeserializeObject<T5>((string) val5),
                DeserializeObject<T6>((string) val6),
                DeserializeObject<T7>((string) val7),
                DeserializeObject<T8>((string) val8));
        }
    }

    public class NetworkMethod<T1, T2, T3, T4, T5, T6, T7, T8, T9> : NetworkMethod
    {
        public NetworkMethod(string eventName, Action<Player, T1, T2, T3, T4, T5, T6, T7, T8, T9> callback = null)
            : base(eventName, callback)
        {
        }

        public void Invoke(
            Player target,
            T1 value1,
            T2 value2,
            T3 value3,
            T4 value4,
            T5 value5,
            T6 value6,
            T7 value7,
            T8 value8,
            T9 value9)
        {
            this.InvokeInternal(
                target,
                SerializeObject(value1),
                SerializeObject(value2),
                SerializeObject(value3),
                SerializeObject(value4),
                SerializeObject(value5),
                SerializeObject(value6),
                SerializeObject(value7),
                SerializeObject(value8),
                SerializeObject(value9));
        }

        protected override Delegate GetRegisterCallback()
        {
            return new Action<Player, object, object, object, object, object, object, object, object, object>(
                this.SerializedCallback);
        }

        private void SerializedCallback(
            [FromSource] Player player,
            object val1,
            object val2,
            object val3,
            object val4,
            object val5,
            object val6,
            object val7,
            object val8,
            object val9)
        {
            this.Callback.DynamicInvoke(
                player,
                DeserializeObject<T1>((string) val1),
                DeserializeObject<T2>((string) val2),
                DeserializeObject<T3>((string) val3),
                DeserializeObject<T4>((string) val4),
                DeserializeObject<T5>((string) val5),
                DeserializeObject<T6>((string) val6),
                DeserializeObject<T7>((string) val7),
                DeserializeObject<T8>((string) val8),
                DeserializeObject<T9>((string) val9));
        }
    }

    public class NetworkMethod<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> : NetworkMethod
    {
        public NetworkMethod(string eventName, Action<Player, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> callback = null)
            : base(eventName, callback)
        {
        }

        public void Invoke(
            Player target,
            T1 value1,
            T2 value2,
            T3 value3,
            T4 value4,
            T5 value5,
            T6 value6,
            T7 value7,
            T8 value8,
            T9 value9,
            T10 value10)
        {
            this.InvokeInternal(
                target,
                SerializeObject(value1),
                SerializeObject(value2),
                SerializeObject(value3),
                SerializeObject(value4),
                SerializeObject(value5),
                SerializeObject(value6),
                SerializeObject(value7),
                SerializeObject(value8),
                SerializeObject(value9),
                SerializeObject(value10));
        }

        protected override Delegate GetRegisterCallback()
        {
            return new Action<Player, object, object, object, object, object, object, object, object, object, object>(
                this.SerializedCallback);
        }

        private void SerializedCallback(
            [FromSource] Player player,
            object val1,
            object val2,
            object val3,
            object val4,
            object val5,
            object val6,
            object val7,
            object val8,
            object val9,
            object val10)
        {
            this.Callback.DynamicInvoke(
                player,
                DeserializeObject<T1>((string) val1),
                DeserializeObject<T2>((string) val2),
                DeserializeObject<T3>((string) val3),
                DeserializeObject<T4>((string) val4),
                DeserializeObject<T5>((string) val5),
                DeserializeObject<T6>((string) val6),
                DeserializeObject<T7>((string) val7),
                DeserializeObject<T8>((string) val8),
                DeserializeObject<T9>((string) val9),
                DeserializeObject<T10>((string) val10));
        }
    }

    public class NetworkMethod<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> : NetworkMethod
    {
        public NetworkMethod(
            string eventName,
            Action<Player, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> callback = null)
            : base(eventName, callback)
        {
        }

        public void Invoke(
            Player target,
            T1 value1,
            T2 value2,
            T3 value3,
            T4 value4,
            T5 value5,
            T6 value6,
            T7 value7,
            T8 value8,
            T9 value9,
            T10 value10,
            T11 value11)
        {
            this.InvokeInternal(
                target,
                SerializeObject(value1),
                SerializeObject(value2),
                SerializeObject(value3),
                SerializeObject(value4),
                SerializeObject(value5),
                SerializeObject(value6),
                SerializeObject(value7),
                SerializeObject(value8),
                SerializeObject(value9),
                SerializeObject(value10),
                SerializeObject(value11));
        }

        protected override Delegate GetRegisterCallback()
        {
            return new Action<Player, object, object, object, object, object, object, object, object, object, object,
                object>(this.SerializedCallback);
        }

        private void SerializedCallback(
            [FromSource] Player player,
            object val1,
            object val2,
            object val3,
            object val4,
            object val5,
            object val6,
            object val7,
            object val8,
            object val9,
            object val10,
            object val11)
        {
            this.Callback.DynamicInvoke(
                player,
                DeserializeObject<T1>((string) val1),
                DeserializeObject<T2>((string) val2),
                DeserializeObject<T3>((string) val3),
                DeserializeObject<T4>((string) val4),
                DeserializeObject<T5>((string) val5),
                DeserializeObject<T6>((string) val6),
                DeserializeObject<T7>((string) val7),
                DeserializeObject<T8>((string) val8),
                DeserializeObject<T9>((string) val9),
                DeserializeObject<T10>((string) val10),
                DeserializeObject<T11>((string) val11));
        }
    }

    public class NetworkMethod<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> : NetworkMethod
    {
        public NetworkMethod(
            string eventName,
            Action<Player, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> callback = null)
            : base(eventName, callback)
        {
        }

        public void Invoke(
            Player target,
            T1 value1,
            T2 value2,
            T3 value3,
            T4 value4,
            T5 value5,
            T6 value6,
            T7 value7,
            T8 value8,
            T9 value9,
            T10 value10,
            T11 value11,
            T12 value12)
        {
            this.InvokeInternal(
                target,
                SerializeObject(value1),
                SerializeObject(value2),
                SerializeObject(value3),
                SerializeObject(value4),
                SerializeObject(value5),
                SerializeObject(value6),
                SerializeObject(value7),
                SerializeObject(value8),
                SerializeObject(value9),
                SerializeObject(value10),
                SerializeObject(value11),
                SerializeObject(value12));
        }

        protected override Delegate GetRegisterCallback()
        {
            return new Action<Player, object, object, object, object, object, object, object, object, object, object,
                object, object>(this.SerializedCallback);
        }

        private void SerializedCallback(
            [FromSource] Player player,
            object val1,
            object val2,
            object val3,
            object val4,
            object val5,
            object val6,
            object val7,
            object val8,
            object val9,
            object val10,
            object val11,
            object val12)
        {
            this.Callback.DynamicInvoke(
                player,
                DeserializeObject<T1>((string) val1),
                DeserializeObject<T2>((string) val2),
                DeserializeObject<T3>((string) val3),
                DeserializeObject<T4>((string) val4),
                DeserializeObject<T5>((string) val5),
                DeserializeObject<T6>((string) val6),
                DeserializeObject<T7>((string) val7),
                DeserializeObject<T8>((string) val8),
                DeserializeObject<T9>((string) val9),
                DeserializeObject<T10>((string) val10),
                DeserializeObject<T11>((string) val11),
                DeserializeObject<T12>((string) val12));
        }
    }

    public class NetworkMethod<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> : NetworkMethod
    {
        public NetworkMethod(
            string eventName,
            Action<Player, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> callback = null)
            : base(eventName, callback)
        {
        }

        public void Invoke(
            Player target,
            T1 value1,
            T2 value2,
            T3 value3,
            T4 value4,
            T5 value5,
            T6 value6,
            T7 value7,
            T8 value8,
            T9 value9,
            T10 value10,
            T11 value11,
            T12 value12,
            T13 value13)
        {
            this.InvokeInternal(
                target,
                SerializeObject(value1),
                SerializeObject(value2),
                SerializeObject(value3),
                SerializeObject(value4),
                SerializeObject(value5),
                SerializeObject(value6),
                SerializeObject(value7),
                SerializeObject(value8),
                SerializeObject(value9),
                SerializeObject(value10),
                SerializeObject(value11),
                SerializeObject(value12),
                SerializeObject(value13));
        }

        protected override Delegate GetRegisterCallback()
        {
            return new Action<Player, object, object, object, object, object, object, object, object, object, object,
                object, object, object>(this.SerializedCallback);
        }

        private void SerializedCallback(
            [FromSource] Player player,
            object val1,
            object val2,
            object val3,
            object val4,
            object val5,
            object val6,
            object val7,
            object val8,
            object val9,
            object val10,
            object val11,
            object val12,
            object val13)
        {
            this.Callback.DynamicInvoke(
                player,
                DeserializeObject<T1>((string) val1),
                DeserializeObject<T2>((string) val2),
                DeserializeObject<T3>((string) val3),
                DeserializeObject<T4>((string) val4),
                DeserializeObject<T5>((string) val5),
                DeserializeObject<T6>((string) val6),
                DeserializeObject<T7>((string) val7),
                DeserializeObject<T8>((string) val8),
                DeserializeObject<T9>((string) val9),
                DeserializeObject<T10>((string) val10),
                DeserializeObject<T11>((string) val11),
                DeserializeObject<T12>((string) val12),
                DeserializeObject<T13>((string) val13));
        }
    }

    public class NetworkMethod<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> : NetworkMethod
    {
        public NetworkMethod(
            string eventName,
            Action<Player, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> callback = null)
            : base(eventName, callback)
        {
        }

        public void Invoke(
            Player target,
            T1 value1,
            T2 value2,
            T3 value3,
            T4 value4,
            T5 value5,
            T6 value6,
            T7 value7,
            T8 value8,
            T9 value9,
            T10 value10,
            T11 value11,
            T12 value12,
            T13 value13,
            T14 value14)
        {
            this.InvokeInternal(
                target,
                SerializeObject(value1),
                SerializeObject(value2),
                SerializeObject(value3),
                SerializeObject(value4),
                SerializeObject(value5),
                SerializeObject(value6),
                SerializeObject(value7),
                SerializeObject(value8),
                SerializeObject(value9),
                SerializeObject(value10),
                SerializeObject(value11),
                SerializeObject(value12),
                SerializeObject(value13),
                SerializeObject(value14));
        }

        protected override Delegate GetRegisterCallback()
        {
            return new Action<Player, object, object, object, object, object, object, object, object, object, object,
                object, object, object, object>(this.SerializedCallback);
        }

        private void SerializedCallback(
            [FromSource] Player player,
            object val1,
            object val2,
            object val3,
            object val4,
            object val5,
            object val6,
            object val7,
            object val8,
            object val9,
            object val10,
            object val11,
            object val12,
            object val13,
            object val14)
        {
            this.Callback.DynamicInvoke(
                player,
                DeserializeObject<T1>((string) val1),
                DeserializeObject<T2>((string) val2),
                DeserializeObject<T3>((string) val3),
                DeserializeObject<T4>((string) val4),
                DeserializeObject<T5>((string) val5),
                DeserializeObject<T6>((string) val6),
                DeserializeObject<T7>((string) val7),
                DeserializeObject<T8>((string) val8),
                DeserializeObject<T9>((string) val9),
                DeserializeObject<T10>((string) val10),
                DeserializeObject<T11>((string) val11),
                DeserializeObject<T12>((string) val12),
                DeserializeObject<T13>((string) val13),
                DeserializeObject<T14>((string) val14));
        }
    }

    public class NetworkMethod<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> : NetworkMethod
    {
        public NetworkMethod(
            string eventName,
            Action<Player, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> callback = null)
            : base(eventName, callback)
        {
        }

        public void Invoke(
            Player target,
            T1 value1,
            T2 value2,
            T3 value3,
            T4 value4,
            T5 value5,
            T6 value6,
            T7 value7,
            T8 value8,
            T9 value9,
            T10 value10,
            T11 value11,
            T12 value12,
            T13 value13,
            T14 value14,
            T15 value15)
        {
            this.InvokeInternal(
                target,
                SerializeObject(value1),
                SerializeObject(value2),
                SerializeObject(value3),
                SerializeObject(value4),
                SerializeObject(value5),
                SerializeObject(value6),
                SerializeObject(value7),
                SerializeObject(value8),
                SerializeObject(value9),
                SerializeObject(value10),
                SerializeObject(value11),
                SerializeObject(value12),
                SerializeObject(value13),
                SerializeObject(value14),
                SerializeObject(value15));
        }

        protected override Delegate GetRegisterCallback()
        {
            return new Action<Player, object, object, object, object, object, object, object, object, object, object,
                object, object, object, object, object>(this.SerializedCallback);
        }

        private void SerializedCallback(
            [FromSource] Player player,
            object val1,
            object val2,
            object val3,
            object val4,
            object val5,
            object val6,
            object val7,
            object val8,
            object val9,
            object val10,
            object val11,
            object val12,
            object val13,
            object val14,
            object val15)
        {
            this.Callback.DynamicInvoke(
                player,
                DeserializeObject<T1>((string) val1),
                DeserializeObject<T2>((string) val2),
                DeserializeObject<T3>((string) val3),
                DeserializeObject<T4>((string) val4),
                DeserializeObject<T5>((string) val5),
                DeserializeObject<T6>((string) val6),
                DeserializeObject<T7>((string) val7),
                DeserializeObject<T8>((string) val8),
                DeserializeObject<T9>((string) val9),
                DeserializeObject<T10>((string) val10),
                DeserializeObject<T11>((string) val11),
                DeserializeObject<T12>((string) val12),
                DeserializeObject<T13>((string) val13),
                DeserializeObject<T14>((string) val14),
                DeserializeObject<T15>((string) val15));
        }
    }
}