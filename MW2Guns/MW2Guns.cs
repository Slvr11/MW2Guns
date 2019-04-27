using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfinityScript;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace MW2Guns
{
    public unsafe class MW2Guns : BaseScript
    {
        private int[] camoOffsets = new int[10];
        private int[] gunOffsets = new int[10];
        private int M16ACOG = 0x2414A450;
        private int M16Holo = 0x2414A451;
        private int M16Grip = 0x2414A452;
        private int M16Sensor = 0x2414A453;
        private int M16GL = 0x2414A456;
        private int M16RDS = 0x2414A457;
        private int M16Shotgun = 0x2414A458;
        private int M16Silencer = 0x2414A459;
        private int M16Iron = 0x2414A45E;
        private int M16Thermal = 0x2414A45F;
        private int AUGACOG = 0x2507077C;
        private int AUGHolo = 0x2507077D;
        private int AUGGrip = 0x25070786;
        private int AUGSensor = 0x2507077E;
        private int AUGScope = 0x2507078B;
        private int AUGRDS = 0x25070784;
        private int AUGSilencer = 0x25070785;
        private int AUGIron = 0x2507078A;
        private int AUGThermal = 0x2507078C;

        [DllImport("kernel32.dll")]
        static extern IntPtr OpenProcess(ProcessAccessFlags dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out int lpNumberOfBytesWritten);

        [DllImport("kernel32.dll")]
        public static extern Int32 CloseHandle(IntPtr hProcess);

        [Flags]
        public enum ProcessAccessFlags : uint
        {
            All = 0x001F0FFF,
            Terminate = 0x00000001,
            CreateThread = 0x00000002,
            VMOperation = 0x00000008,
            VMRead = 0x00000010,
            VMWrite = 0x00000020,
            DupHandle = 0x00000040,
            SetInformation = 0x00000200,
            QueryInformation = 0x00000400,
            Synchronize = 0x00100000
        }

        public static void WriteMem(Process p, int address, long v)
        {
            var hProc = OpenProcess(ProcessAccessFlags.All, false, (int)p.Id);
            var val = new byte[] { (byte)v };

            int wtf = 0;
            WriteProcessMemory(hProc, new IntPtr(address), val, (UInt32)val.LongLength, out wtf);

            CloseHandle(hProc);
        }

        public MW2Guns()
        {
            camoOffsets[0] = 0x1AC2375;
            camoOffsets[1] = camoOffsets[0] + 0x38AC;
            camoOffsets[2] = camoOffsets[1] + 0x38AC;
            camoOffsets[3] = camoOffsets[2] + 0x38AC;
            camoOffsets[4] = camoOffsets[3] + 0x38AC;
            camoOffsets[5] = camoOffsets[4] + 0x38AC;
            camoOffsets[6] = camoOffsets[5] + 0x38AC;
            camoOffsets[7] = camoOffsets[6] + 0x386C;
            camoOffsets[8] = camoOffsets[7] + 0x38AC;
            camoOffsets[9] = camoOffsets[8] + 0x38AC;
            gunOffsets[0] = 0x1AC2374;
            gunOffsets[1] = gunOffsets[0] + 0x38AC;
            gunOffsets[2] = gunOffsets[1] + 0x38AC;
            gunOffsets[3] = gunOffsets[2] + 0x38AC;
            gunOffsets[4] = gunOffsets[3] + 0x38AC;
            gunOffsets[5] = gunOffsets[4] + 0x38AC;
            gunOffsets[6] = gunOffsets[5] + 0x38AC;
            gunOffsets[7] = gunOffsets[6] + 0x386C;
            gunOffsets[8] = gunOffsets[7] + 0x38AC;
            gunOffsets[9] = gunOffsets[8] + 0x38AC;
            PlayerConnected += new Action<Entity>(entity =>
            {
                entity.SetField("augCamo", 0);
                entity.OnNotify("weapon_change", (player, newWeap) =>
                {
                    AfterDelay(200, () =>
                        {
                            if (newWeap.As<string>().Contains("iw5_m60jugg_mp") && newWeap.As<string>().Contains("_camo"))
                            {
                                if (newWeap.As<string>().Split('_').Length == 4)
                                {
                                    SetMW2AUG(entity, "", "", entity.GetWeaponAmmoClip(newWeap.As<string>()), entity.GetWeaponAmmoStock(newWeap.As<string>()));
                                }
                                else if (newWeap.As<string>().Split('_').Length == 5)
                                {
                                    SetMW2AUG(entity, newWeap.As<string>().Split('_')[3], "", entity.GetWeaponAmmoClip(newWeap.As<string>()), entity.GetWeaponAmmoStock(newWeap.As<string>()));
                                }
                                else if (newWeap.As<string>().Split('_').Length > 5)
                                {
                                    SetMW2AUG(entity, newWeap.As<string>().Split('_')[3], newWeap.As<string>().Split('_')[4], entity.GetWeaponAmmoClip(newWeap.As<string>()), entity.GetWeaponAmmoStock(newWeap.As<string>()));
                                }
                            }
                            else if (newWeap.As<string>().Contains("iw5_m16_mp") && !newWeap.As<string>().Contains("_camo"))
                            {
                                if (newWeap.As<string>().Split('_').Length == 3)
                                {
                                    SetMW2M16(entity, "", "", entity.GetWeaponAmmoClip(newWeap.As<string>()), entity.GetWeaponAmmoStock(newWeap.As<string>()));
                                }
                                else if (newWeap.As<string>().Split('_').Length == 4)
                                {
                                    SetMW2M16(entity, newWeap.As<string>().Split('_')[3], "", entity.GetWeaponAmmoClip(newWeap.As<string>()), entity.GetWeaponAmmoStock(newWeap.As<string>()));
                                }
                                else if (newWeap.As<string>().Split('_').Length > 4)
                                {
                                    SetMW2M16(entity, newWeap.As<string>().Split('_')[3], newWeap.As<string>().Split('_')[4], entity.GetWeaponAmmoClip(newWeap.As<string>()), entity.GetWeaponAmmoStock(newWeap.As<string>()));
                                }
                            }
                            /*
                            else if (newWeap.As<string>() != "gl_mp" && !newWeap.As<string>().Contains("_camo15") && (!newWeap.As<string>().Contains("iw5_m16_mp") && !newWeap.As<string>().Contains("iw5_sa80_mp")))
                            {
                                ResetM16Attachments();
                                ResetAUGAttachments();
                                entity.SetClientDvar("cg_gun_z", "0");
                                entity.SetClientDvar("cg_gun_x", "0");
                                if (*(byte*)gunOffsets[entity.EntRef] == 0x19)
                                *(byte*)camoOffsets[entity.EntRef] = (byte)(*(byte*)camoOffsets[entity.EntRef] - 15);
                                else if (*(byte*)gunOffsets[entity.EntRef] == 0x39)
                                {
                                    *(byte*)gunOffsets[entity.EntRef] = 0x35;
                                    *(byte*)camoOffsets[entity.EntRef] = (byte)entity.GetField<int>("augCamo");
                                }
                            }
                            */
                        });
                });
                entity.SpawnedPlayer += () => OnPlayerSpawned(entity);
            });
        }
        public override void OnSay(Entity player, string name, string message)
        {
            if (message.StartsWith("z"))
                player.SetClientDvar("cg_gun_z", message.Split(' ')[1]);
            if (message.StartsWith("x"))
                player.SetClientDvar("cg_gun_x", message.Split(' ')[1]);
            if (message.StartsWith("tst"))
                Log.Write(LogLevel.All, "{0}", Utilities.GetAttachmentType(message.Split(' ')[1]));
            if (message.StartsWith ("give "))
            {
                string weapon = message.Split(' ')[1];
                player.GiveWeapon(weapon);
                player.SwitchToWeapon(weapon);
            }
        }
        public override void OnPlayerKilled(Entity player, Entity inflictor, Entity attacker, int damage, string mod, string weapon, Vector3 dir, string hitLoc)
        {
            if (player.CurrentWeapon.Contains("_camo15"))
                player.TakeAllWeapons();
            ResetM16Attachments();
            ResetAUGAttachments();
        }
        public override void OnPlayerDisconnect(Entity player)
        {
            ResetAUGAttachments();
            ResetM16Attachments();
        }
        public override void OnExitLevel()
        {
            ResetM16Attachments();
            ResetAUGAttachments();
        }
        public void OnPlayerSpawned(Entity player)
        {
            player.GiveWeapon("gl_mp");
        }
        public void SetMW2M16(Entity player, string attach1, string attach2, int clipAmmo, int stockAmmo)
        {
            int ID = player.EntRef;
            player.SwitchToWeaponImmediate("gl_mp");
            /*
            if (attach1 == "acog" || attach2 == "acog") *(int*)camoOffsets[ID] = *(int*)camoOffsets[ID] + 0x0F;
            else if (attach1 == "eotech" || attach2 == "eotech") *(int*)camoOffsets[ID] = *(int*)camoOffsets[ID] + 0x0F;
            else if (attach1 == "reflex" || attach2 == "reflex") *(int*)camoOffsets[ID] = *(int*)camoOffsets[ID] + 0x0F;
            else if (attach1 == "thermal" || attach2 == "thermal") *(int*)camoOffsets[ID] = *(int*)camoOffsets[ID] + 0x0F;
            else if (attach1 == "shotgun" || attach2 == "shotgun") *(int*)camoOffsets[ID] = *(int*)camoOffsets[ID] + 0x0F;
             */
            *(int*)camoOffsets[ID] = *(int*)camoOffsets[ID] + 0x0F;
            AfterDelay(100, () =>
                {
                    Log.Write(LogLevel.All, attach1 + " and " + attach2);
                    if (attach1 != "" && attach2 != "")
                    {
                        player.SwitchToWeaponImmediate("iw5_m16_mp_" + attach1 + "_" + attach2 + "_camo15");
                        player.SetWeaponAmmoClip("iw5_m16_mp_" + attach1 + "_" + attach2 + "_camo15", clipAmmo);
                        player.SetWeaponAmmoStock("iw5_m16_mp_" + attach1 + "_" + attach2 + "_camo15", stockAmmo);
                    }
                    else if (attach1 != "")
                    {
                        player.SwitchToWeaponImmediate("iw5_m16_mp_" + attach1 + "_camo15");
                        player.SetWeaponAmmoClip("iw5_m16_mp_" + attach1 + "_camo15", clipAmmo);
                        player.SetWeaponAmmoStock("iw5_m16_mp_" + attach1 + "_camo15", stockAmmo);
                    }
                    else
                    {
                        player.SwitchToWeaponImmediate("iw5_m16_mp_camo15");
                        player.SetWeaponAmmoClip("iw5_m16_mp_camo15", clipAmmo);
                        player.SetWeaponAmmoStock("iw5_m16_mp_camo15", stockAmmo);
                    }
                    AfterDelay(100, () =>
                            ClearM16Attachments(player, attach1, attach2));
                });
        }
        public void SetMW2AUG(Entity player, string attach1, string attach2, int clipAmmo, int stockAmmo)
        {
            int ID = player.EntRef;
            player.SetField("augCamo", *(byte*)camoOffsets[ID]);
            if (attach1 == "acog" || attach2 == "acog") *(byte*)camoOffsets[ID] = 0x1F;
            else if (attach1 == "eotechlmg" || attach2 == "eotechlmg") *(byte*)camoOffsets[ID] = 0x2F;
            else if (attach1 == "reflexlmg" || attach2 == "reflexlmg") *(byte*)camoOffsets[ID] = 0x3F;
            else if (attach1 == "thermal" || attach2 == "thermal") *(byte*)camoOffsets[ID] = 0x4F;
            else if (attach1 == "grip" || attach2 == "grip") *(byte*)camoOffsets[ID] = 0x8F;
            else *(byte*)camoOffsets[ID] = 0x0F;
            *(byte*)gunOffsets[ID] = 0x39;
            AfterDelay(100, () =>
            {
                Log.Write(LogLevel.All, attach1 + " and " + attach2);
                if (attach1 != "" && attach2 != "")
                {
                    player.SwitchToWeaponImmediate("iw5_sa80_mp_" + attach1 + "_" + attach2 + "_camo15");
                    player.SetWeaponAmmoClip("iw5_sa80_mp_" + attach1 + "_" + attach2 + "_camo15", clipAmmo);
                    player.SetWeaponAmmoStock("iw5_sa80_mp_" + attach1 + "_" + attach2 + "_camo15", stockAmmo);
                }
                else if (attach1 != "")
                {
                    player.SwitchToWeaponImmediate("iw5_sa80_mp_" + attach1 + "_camo15");
                    player.SetWeaponAmmoClip("iw5_sa80_mp_" + attach1 + "_camo15", clipAmmo);
                    player.SetWeaponAmmoStock("iw5_sa80_mp_" + attach1 + "_camo15", stockAmmo);
                }
                else
                {
                    player.SwitchToWeaponImmediate("iw5_sa80_mp_camo15");
                    player.SetWeaponAmmoClip("iw5_sa80_mp_camo15", clipAmmo);
                    player.SetWeaponAmmoStock("iw5_sa80_mp_camo15", stockAmmo);
                }
                AfterDelay(100, () =>
                        ClearAUGAttachments(player, attach1, attach2));
            });
        }
        public void ClearM16Attachments(Entity player, string attach1, string attach2)
        {
            var p = Process.GetProcessesByName("iw5mp").FirstOrDefault();
            WriteMem(p, M16ACOG, 0x0);
            WriteMem(p, M16Holo, 0x0);
            WriteMem(p, M16Grip, 0x0);
            WriteMem(p, M16Sensor, 0x0);
            WriteMem(p, M16GL, 0x0);
            WriteMem(p, M16RDS, 0x0);
            WriteMem(p, M16Shotgun, 0x0);
            WriteMem(p, M16Silencer, 0x0);
            //WriteMem(p, M16Iron, 0x0);
            WriteMem(p, M16Thermal, 0x0);
            if (Utilities.GetAttachmentType(attach1) == "rail")
            {
                SetM16Attachment(player, attach2);
                SetM16Attachment(player, attach1);
            }
            else
            {
                SetM16Attachment(player, attach1);
                SetM16Attachment(player, attach2);
            }
        }
        public void ClearAUGAttachments(Entity player, string attach1, string attach2)
        {
            var p = Process.GetProcessesByName("iw5mp").FirstOrDefault();
            WriteMem(p, AUGACOG, 0x0);
            WriteMem(p, AUGHolo, 0x0);
            WriteMem(p, AUGGrip, 0x0);
            WriteMem(p, AUGSensor, 0x0);
            WriteMem(p, AUGScope, 0x0);
            WriteMem(p, AUGRDS, 0x0);
            WriteMem(p, AUGSilencer, 0x0);
            //WriteMem(p, AUGIron, 0x0);
            WriteMem(p, AUGThermal, 0x0);
            if (Utilities.GetAttachmentType(attach1) == "")
            {
                SetAUGAttachment(player, attach2);
                SetAUGAttachment(player, attach1);
            }
            else
            {
                SetAUGAttachment(player, attach1);
                SetAUGAttachment(player, attach2);
            }
        }
        public void ResetM16Attachments()
        {
            var p = Process.GetProcessesByName("iw5mp").FirstOrDefault();
            WriteMem(p, M16ACOG, 0x01);
            WriteMem(p, M16Holo, 0x02);
            WriteMem(p, M16Grip, 0x03);
            WriteMem(p, M16Sensor, 0x04);
            WriteMem(p, M16GL, 0x07);
            WriteMem(p, M16RDS, 0x08);
            WriteMem(p, M16Shotgun, 0x09);
            WriteMem(p, M16Silencer, 0x0A);
            WriteMem(p, M16Iron, 0x0F);
            WriteMem(p, M16Thermal, 0x10);
        }
        public void ResetAUGAttachments()
        {
            var p = Process.GetProcessesByName("iw5mp").FirstOrDefault();
            WriteMem(p, AUGACOG, 0x01);
            WriteMem(p, AUGHolo, 0x02);
            WriteMem(p, AUGGrip, 0x0B);
            WriteMem(p, AUGSensor, 0x03);
            WriteMem(p, AUGScope, 0x10);
            WriteMem(p, AUGRDS, 0x09);
            WriteMem(p, AUGSilencer, 0x0A);
            WriteMem(p, AUGIron, 0x0F);
            WriteMem(p, AUGThermal, 0x11);
        }
        public void SetM16Attachment(Entity player, string attach)
        {
            Log.Write(LogLevel.All, "Writing attachment {0} in memory", attach);
            var p = Process.GetProcessesByName("iw5mp").FirstOrDefault();
            switch (attach)
            {
                case "acog":
                    WriteMem(p, M16ACOG, 0x01);
                    WriteMem(p, M16Iron, 0x00);
                    player.SetClientDvar("cg_gun_z", "0.55");
                    break;
                case "eotech":
                    WriteMem(p, M16Holo, 0x02);
                    WriteMem(p, M16Iron, 0x00);
                    player.SetClientDvar("cg_gun_z", "0.7");
                    break;
                case "heartbeat":
                    WriteMem(p, M16Sensor, 0x04);
                    WriteMem(p, M16Iron, 0x0F);
                    player.SetClientDvar("cg_gun_z", "-0.15");
                    break;
                case "gl":
                    WriteMem(p, M16GL, 0x07);
                    WriteMem(p, M16Iron, 0x0F);
                    player.SetClientDvar("cg_gun_z", "-0.15");
                    break;
                case "reflex":
                    WriteMem(p, M16RDS, 0x08);
                    WriteMem(p, M16Iron, 0x00);
                    player.SetClientDvar("cg_gun_z", "0.7");
                    break;
                case "shotgun":
                    WriteMem(p, M16Shotgun, 0x09);
                    WriteMem(p, M16Iron, 0x0F);
                    player.SetClientDvar("cg_gun_z", "-0.15");
                    break;
                case "silencer":
                    WriteMem(p, M16Silencer, 0x0A);
                    WriteMem(p, M16Iron, 0x0F);
                    player.SetClientDvar("cg_gun_z", "-0.15");
                    break;
                case "thermal":
                    WriteMem(p, M16Thermal, 0x10);
                    WriteMem(p, M16Iron, 0x00);
                    player.SetClientDvar("cg_gun_z", "0.65");
                    break;
                case "":
                    //WriteMem(p, M16Iron, 0x0F);
                    player.SetClientDvar("cg_gun_z", "-0.15");
                    break;
            }
        }
        public void SetAUGAttachment(Entity player, string attach)
        {
            Log.Write(LogLevel.All, "Writing attachment {0} in memory", attach);
            var p = Process.GetProcessesByName("iw5mp").FirstOrDefault();
            switch (attach)
            {
                case "acog":
                    WriteMem(p, AUGACOG, 0x01);
                    WriteMem(p, AUGIron, 0x00);
                    player.SetClientDvar("cg_gun_z", "-1.4");
                    player.SetClientDvar("cg_gun_x", "0");
                    break;
                case "eotechlmg":
                    WriteMem(p, AUGHolo, 0x02);
                    WriteMem(p, AUGIron, 0x00);
                    player.SetClientDvar("cg_gun_z", "-1.4");
                    player.SetClientDvar("cg_gun_x", "0");
                    break;
                case "heartbeat":
                    WriteMem(p, AUGSensor, 0x03);
                    WriteMem(p, AUGIron, 0x0F);
                    player.SetClientDvar("cg_gun_z", "0.65");
                    player.SetClientDvar("cg_gun_x", "-4");
                    break;
                case "scope":
                    WriteMem(p, AUGScope, 0x10);
                    WriteMem(p, AUGIron, 0x0F);
                    player.SetClientDvar("cg_gun_z", "-0");
                    break;
                case "reflexlmg":
                    WriteMem(p, AUGRDS, 0x09);
                    WriteMem(p, AUGIron, 0x00);
                    player.SetClientDvar("cg_gun_z", "-1.3");
                    player.SetClientDvar("cg_gun_x", "0");
                    break;
                case "silencer":
                    WriteMem(p, AUGSilencer, 0x0A);
                    WriteMem(p, AUGIron, 0x0F);
                    player.SetClientDvar("cg_gun_z", "0.65");
                    player.SetClientDvar("cg_gun_x", "-4");
                    break;
                case "thermal":
                    WriteMem(p, AUGThermal, 0x11);
                    WriteMem(p, AUGIron, 0x00);
                    player.SetClientDvar("cg_gun_z", "-1.3");
                    player.SetClientDvar("cg_gun_x", "0");
                    break;
                case "grip":
                    WriteMem(p, AUGGrip, 0x0B);
                    WriteMem(p, AUGIron, 0x0F);
                    player.SetClientDvar("cg_gun_z", "0.65");
                    player.SetClientDvar("cg_gun_x", "-4");
                    break;
                case "":
                    //WriteMem(p, M16Iron, 0x0F);
                    player.SetClientDvar("cg_gun_z", "0.65");
                    player.SetClientDvar("cg_gun_x", "-4");
                    break;
            }
        }
    }
}
