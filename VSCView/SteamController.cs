﻿using HidLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace VSCView
{
    public class SteamController
    {
        private const int VendorId = 0x28DE; // 10462
        private const int ProductIdWireless = 0x1142; // 4418;
        private const int ProductIdWired = 0x1102; // 4354

        //private bool _attached = false;
        private HidDevice _device;

        public enum VSCEventType
        {
            CONTROL_UPDATE = 0x01,
            CONNECTION_DETAIL = 0x03,
            BATTERY_UPDATE = 0x04,
        }

        public enum ConnectionState
        {
            DISCONNECT = 0x01,
            CONNECT = 0x02,
            PAIRING = 0x03,
        }

        public class SteamControllerButtons : ICloneable
        {
            public bool A { get; set; }
            public bool B { get; set; }
            public bool X { get; set; }
            public bool Y { get; set; }

            public bool LeftBumper { get; set; }
            public bool LeftTrigger { get; set; }

            public bool RightBumper { get; set; }
            public bool RightTrigger { get; set; }

            public bool LeftGrip { get; set; }
            public bool RightGrip { get; set; }

            public bool Start { get; set; }
            public bool Steam { get; set; }
            public bool Select { get; set; }

            public bool Down { get; set; }
            public bool Left { get; set; }
            public bool Right { get; set; }
            public bool Up { get; set; }

            public bool StickClick { get; set; }
            public bool LeftPadTouch { get; set; }
            public bool LeftPadClick { get; set; }
            public bool RightPadTouch { get; set; }
            public bool RightPadClick { get; set; }

            public object Clone()
            {
                SteamControllerButtons buttons = new SteamControllerButtons();

                buttons.A = A;
                buttons.B = B;
                buttons.X = X;
                buttons.Y = Y;

                buttons.LeftBumper = LeftBumper;
                buttons.LeftTrigger = LeftTrigger;

                buttons.RightBumper = RightBumper;
                buttons.RightTrigger = RightTrigger;

                buttons.LeftGrip = LeftGrip;
                buttons.RightGrip = RightGrip;

                buttons.Start = Start;
                buttons.Steam = Steam;
                buttons.Select = Select;

                buttons.Down = Down;
                buttons.Left = Left;
                buttons.Right = Right;
                buttons.Up = Up;

                buttons.StickClick = StickClick;
                buttons.LeftPadTouch = LeftPadTouch;
                buttons.LeftPadClick = LeftPadClick;
                buttons.RightPadTouch = RightPadTouch;
                buttons.RightPadClick = RightPadClick;

                return buttons;
            }
        }

        public class SteamControllerState
        {
            public SteamControllerButtons Buttons;

            public byte LeftTrigger { get; set; }
            public byte RightTrigger { get; set; }

            public Int32 LeftStickX { get; set; }
            public Int32 LeftStickY { get; set; }
            public Int32 LeftPadX { get; set; }
            public Int32 LeftPadY { get; set; }
            public Int32 RightPadX { get; set; }
            public Int32 RightPadY { get; set; }

            public Int16 AngularVelocityX { get; set; }
            public Int16 AngularVelocityY { get; set; }
            public Int16 AngularVelocityZ { get; set; }
            public Int16 OrientationW { get; set; }
            public Int16 OrientationX { get; set; }
            public Int16 OrientationY { get; set; }
            public Int16 OrientationZ { get; set; }
        }

        SteamControllerButtons Buttons { get; set; }

        byte LeftTrigger { get; set; }
        byte RightTrigger { get; set; }

        Int32 LeftStickX { get; set; }
        Int32 LeftStickY { get; set; }
        Int32 LeftPadX { get; set; }
        Int32 LeftPadY { get; set; }
        Int32 RightPadX { get; set; }
        Int32 RightPadY { get; set; }

        Int16 AngularVelocityX { get; set; }
        Int16 AngularVelocityY { get; set; }
        Int16 AngularVelocityZ { get; set; }
        Int16 OrientationW { get; set; }
        Int16 OrientationX { get; set; }
        Int16 OrientationY { get; set; }
        Int16 OrientationZ { get; set; }

        bool Initalized;

        public delegate void StateUpdatedEventHandler(object sender, SteamControllerState e);
        public event StateUpdatedEventHandler StateUpdated;
        protected virtual void OnStateUpdated(SteamControllerState e)
        {
            StateUpdated?.Invoke(this, e);
        }

        object controllerStateLock = new object();

        public SteamController(HidDevice device)
        {
            Buttons = new SteamControllerButtons();

            _device = device;

            Initalized = false;
        }

        public void Initalize()
        {
            if (Initalized) return;

            //_device.OpenDevice();

            //_device.Inserted += DeviceAttachedHandler;
            //_device.Removed += DeviceRemovedHandler;

            //_device.MonitorDeviceEvents = true;

            Initalized = true;

            //_attached = _device.IsConnected;

            _device.ReadReport(OnReport);
        }

        public void DeInitalize()
        {
            if (!Initalized) return;

            //_device.Inserted -= DeviceAttachedHandler;
            //_device.Removed -= DeviceRemovedHandler;

            //_device.MonitorDeviceEvents = false;

            Initalized = false;
        }

        public string GetDevicePath()
        {
            return _device.DevicePath;
        }

        public SteamControllerState GetState()
        {
            lock (controllerStateLock)
            {
                SteamControllerState state = new SteamControllerState();
                state.Buttons = (SteamControllerButtons)Buttons.Clone();

                state.LeftTrigger = LeftTrigger;
                state.RightTrigger = RightTrigger;

                state.LeftStickX = LeftStickX;
                state.LeftStickY = LeftStickY;
                state.LeftPadX = LeftPadX;
                state.LeftPadY = LeftPadY;
                state.RightPadX = RightPadX;
                state.RightPadY = RightPadY;

                state.AngularVelocityX = AngularVelocityX;
                state.AngularVelocityY = AngularVelocityY;
                state.AngularVelocityZ = AngularVelocityZ;
                state.OrientationW = OrientationW;
                state.OrientationX = OrientationX;
                state.OrientationY = OrientationY;
                state.OrientationZ = OrientationZ;

                return state;
            }
        }

        public static SteamController[] GetControllers()
        {
            List<HidDevice> _devices = HidDevices.Enumerate(VendorId, ProductIdWireless, ProductIdWired).ToList();
            //Dictionary<int, HidDevice> HidDeviceList = new Dictionary<int, HidDevice>();

            List<HidDevice> HidDeviceList = new List<HidDevice>();

            // we should never have holes, this entire dictionary is just because I don't know if I can trust the order I get the HID devices
            foreach (HidDevice _device in _devices)
            {
                if (_device != null)
                {
                    int index = -1;
                    Match m = Regex.Match(_device.DevicePath, "&mi_([0-9]{2})");
                    if (!m.Success) continue;
                    index = int.Parse(m.Groups[1].Value) - 1;
                    if (index < 0) continue;

                    //HidDeviceList.Add(index, _device);
                    HidDeviceList.Add(_device);
                }
            }

            //SteamController[] Controllers = new SteamController[HidDeviceList.Count];
            //for (int idx = 0; idx < HidDeviceList.Count; idx++)
            //{
            //    if (!HidDeviceList.ContainsKey(idx)) continue;
            //
            //    Controllers[idx] = new SteamController(HidDeviceList[idx]);
            //}
            //
            //return Controllers;

            return HidDeviceList.Select(dr => new SteamController(dr)).ToArray();
        }

        private void OnReport(HidReport report)
        {
            if (!Initalized) return;

            lock (controllerStateLock)
            {
                //SteamControllerState OldState = GetState();

                //if (_attached == false) { return; }

                byte Unknown1 = report.Data[0]; // always 0x01?
                byte Unknown2 = report.Data[1]; // always 0x00?
                VSCEventType EventType = (VSCEventType)report.Data[2];

                switch (EventType)
                {
                    case 0: // not sure what this is but wired controllers do it
                        break;
                    case VSCEventType.CONTROL_UPDATE:
                        {
                            //report.Data[3] // 0x3C?

                            UInt32 PacketIndex = BitConverter.ToUInt32(report.Data, 4);

                            Buttons.A = (report.Data[8] & 128) == 128;
                            Buttons.X = (report.Data[8] & 64) == 64;
                            Buttons.B = (report.Data[8] & 32) == 32;
                            Buttons.Y = (report.Data[8] & 16) == 16;
                            Buttons.LeftBumper = (report.Data[8] & 8) == 8;
                            Buttons.RightBumper = (report.Data[8] & 4) == 4;
                            Buttons.LeftTrigger = (report.Data[8] & 2) == 2;
                            Buttons.RightTrigger = (report.Data[8] & 1) == 1;

                            Buttons.LeftGrip = (report.Data[9] & 128) == 128;
                            Buttons.Start = (report.Data[9] & 64) == 64;
                            Buttons.Steam = (report.Data[9] & 32) == 32;
                            Buttons.Select = (report.Data[9] & 16) == 16;

                            Buttons.Down = (report.Data[9] & 8) == 8;
                            Buttons.Left = (report.Data[9] & 4) == 4;
                            Buttons.Right = (report.Data[9] & 2) == 2;
                            Buttons.Up = (report.Data[9] & 1) == 1;

                            bool LeftAnalogMultiplexMode = (report.Data[10] & 128) == 128;
                            Buttons.StickClick = (report.Data[10] & 64) == 64;
                            bool Unknown = (report.Data[10] & 32) == 32; // what is this?
                            Buttons.RightPadTouch = (report.Data[10] & 16) == 16;
                            bool LeftPadTouch = (report.Data[10] & 8) == 8;
                            Buttons.RightPadClick = (report.Data[10] & 4) == 4;
                            bool ThumbOrLeftPadPress = (report.Data[10] & 2) == 2; // what is this even for?
                            Buttons.RightGrip = (report.Data[10] & 1) == 1;

                            LeftTrigger = report.Data[11];
                            RightTrigger = report.Data[12];

                            if (LeftAnalogMultiplexMode)
                            {
                                if (LeftPadTouch)
                                {
                                    Buttons.LeftPadTouch = true;
                                    Buttons.LeftPadClick = ThumbOrLeftPadPress;
                                    LeftPadX = BitConverter.ToInt16(report.Data, 16);
                                    LeftPadY = BitConverter.ToInt16(report.Data, 18);
                                }
                                else
                                {
                                    LeftStickX = BitConverter.ToInt16(report.Data, 16);
                                    LeftStickY = BitConverter.ToInt16(report.Data, 18);
                                }
                            }
                            else
                            {
                                if (LeftPadTouch)
                                {
                                    Buttons.LeftPadTouch = true;
                                    LeftPadX = BitConverter.ToInt16(report.Data, 16);
                                    LeftPadY = BitConverter.ToInt16(report.Data, 18);
                                }
                                else
                                {
                                    Buttons.LeftPadTouch = false;
                                    LeftStickX = BitConverter.ToInt16(report.Data, 16);
                                    LeftStickY = BitConverter.ToInt16(report.Data, 18);
                                    LeftPadX = 0;
                                    LeftPadY = 0;
                                }

                                Buttons.LeftPadClick = ThumbOrLeftPadPress && !Buttons.StickClick;
                            }

                            RightPadX = BitConverter.ToInt16(report.Data, 20);
                            RightPadY = BitConverter.ToInt16(report.Data, 22);

                            /*
                            //NiceOutputText[28] += " -------- Acceleration X: " + BitConverter.ToInt16(report.Data, 28);
                            //NiceOutputText[29] += " ^^^^^^^^";
                            //NiceOutputText[30] += " -------- Acceleration Y: " + BitConverter.ToInt16(report.Data, 30);
                            //NiceOutputText[31] += " ^^^^^^^^";
                            //NiceOutputText[32] += " -------- Acceleration Z: " + BitConverter.ToInt16(report.Data, 32);
                            //NiceOutputText[33] += " ^^^^^^^^";
                            */
                            AngularVelocityX = BitConverter.ToInt16(report.Data, 34);
                            AngularVelocityY = BitConverter.ToInt16(report.Data, 36);
                            AngularVelocityZ = BitConverter.ToInt16(report.Data, 38);
                            OrientationW = BitConverter.ToInt16(report.Data, 40);
                            OrientationX = BitConverter.ToInt16(report.Data, 42);
                            OrientationY = BitConverter.ToInt16(report.Data, 44);
                            OrientationZ = BitConverter.ToInt16(report.Data, 46);
                        }
                        break;

                    case VSCEventType.CONNECTION_DETAIL:
                        {
                            //report.Data[3] // 0x01?

                            // Connection detail. 0x01 for disconnect, 0x02 for connect, 0x03 for pairing request.
                            ConnectionState ConnectionStateV = (ConnectionState)report.Data[4];

                            if (report.Data[4] == 0x01)
                            {
                                byte[] tmpBytes = new byte[4];
                                tmpBytes[1] = report.Data[5];
                                tmpBytes[2] = report.Data[6];
                                tmpBytes[3] = report.Data[7];

                                //BitConverter.ToUInt32(tmpBytes, 0); // Timestamp
                            }
                        }
                        break;

                    case VSCEventType.BATTERY_UPDATE:
                        {
                            //report.Data[3] // 0x0B?

                            UInt32 PacketIndex = BitConverter.ToUInt32(report.Data, 4);

                            // only works if controller is configured to send this data

                            // millivolts
                            UInt16 BatteryVoltage = BitConverter.ToUInt16(report.Data, 8);
                            //BitConverter.ToUInt16(report.Data, 10); // UNKNOWN, stuck at 100
                        }
                        break;

                    default:
                        {
                            Console.WriteLine("Unknown Packet Type " + EventType);
                        }
                        break;
                }

                SteamControllerState NewState = GetState();
                OnStateUpdated(NewState);

                _device.ReadReport(OnReport);
            }
        }

        /*private void DeviceAttachedHandler()
        {
            lock (controllerStateLock)
            {
                _attached = true;
                Console.WriteLine("VSC Address Attached");
                _device.ReadReport(OnReport);
            }
        }

        private void DeviceRemovedHandler()
        {
            lock (controllerStateLock)
            {
                _attached = false;
                Console.WriteLine("VSC Address Removed");
            }
        }*/
    }
}
