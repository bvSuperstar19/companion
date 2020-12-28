using ExileCore;
using ExileCore.PoEMemory.Components;
using ExileCore.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharpDX.Mathematics;
using ExileCore.PoEMemory.MemoryObjects;


namespace Companion {
    public class Companion : BaseSettingsPlugin<CompanionSetting> {
        private DateTime _lastCast = DateTime.MinValue;




        private void OnToggleSSkillSetNodeChange() {
            Input.RegisterKey(this.Settings.ToggleSkillSetNode);
        }

        public override bool Initialise() {

            Input.RegisterKey(this.Settings.ToggleSkillSetNode);
            this.Settings.ToggleSkillSetNode.OnValueChanged += OnToggleSSkillSetNodeChange;
            return true;
        }

        public override Job Tick() {
            var localPlayer = this.GameController.Game.IngameState.Data.LocalPlayer;
            var entities = this.GameController.Entities;
            var buffs = localPlayer.Buffs;
            var life = localPlayer.GetComponent<Life>();



            if (!this.Settings.Enable) {
                return null;
            }

            #region Smoke Mine

            SharpDX.Vector3 smokeMineLastPosition = new SharpDX.Vector3(0, 0, 0);

            if (Settings.smokeMineEnabled) {
                var mines = localPlayer.GetComponent<Actor>().DeployedObjects.Where(x => x.Entity != null && x.Entity.Path == "Metadata/MiscellaneousObjects/RemoteMine").ToList();
                if (mines.Count > 0) {
                    if (mines.Any(x => x.Entity.Pos == smokeMineLastPosition)) {
                        Input.KeyDown(this.Settings.HotKeyDetonateMine);
                        Input.KeyUp(this.Settings.HotKeyDetonateMine);
                        smokeMineLastPosition = new SharpDX.Vector3(0, 0, 0);
                    }
                    else {
                        smokeMineLastPosition = mines.First().Entity.Pos
                    }

                    //mines.ForEach(x => DebugWindow.LogMsg(x.Entity.ToString()));
                }

            }

            #endregion


            #region Plague Bearer

            if (this.Settings.plagueBearerEnabled) {

                // Returns null if not enough mana
                if (this.Settings.MinimumMana > life.CurMana) {
                    return null;
                }
                // Return null if cool down span
                TimeSpan cooldownSpan = TimeSpan.FromMilliseconds(this.Settings.CoolDown);
                if (_lastCast + cooldownSpan > DateTime.UtcNow) {
                    return null;
                }

                bool enemiesNearby = false;
                foreach (var entity in entities) {

                    if (entity.IsAlive && entity.IsHostile && entity.IsTargetable) {

                        float distance = (localPlayer.Pos - entity.Pos).Length();

                        if (distance <= this.Settings.Range) {
                            if (entity.Rarity == ExileCore.Shared.Enums.MonsterRarity.White && this.Settings.WhiteMonster == false) {
                                //DebugWindow.LogMsg("reached Normal");
                                continue;
                            }
                            else if (entity.Rarity == ExileCore.Shared.Enums.MonsterRarity.Magic && this.Settings.MagiceMonster == false) {
                                //DebugWindow.LogMsg("reached Magic");
                                continue;

                            }
                            else if (entity.Rarity == ExileCore.Shared.Enums.MonsterRarity.Rare && this.Settings.RareMonster == false) {
                                //DebugWindow.LogMsg("reached Rare");
                                continue;
                            }
                            else if (entity.Rarity == ExileCore.Shared.Enums.MonsterRarity.Unique && this.Settings.UniqueMonster == false) {
                                //DebugWindow.LogMsg("reached Unique");
                                continue;
                            }

                            enemiesNearby = true;
                        }
                    }
                }

                bool auraOn = buffs.Any(b => b.Name == "corrosive_shroud_aura");


                bool switchState = false;
                if (enemiesNearby) {
                    // Turn aura on if enemies nearby
                    if (!auraOn) {
                        switchState = true;
                    }
                }
                else {
                    // Turn off aura if enemies not nearby
                    if (auraOn) {
                        switchState = true;
                    }
                }

                if (switchState) {
                    if (this.Settings.HotKey.Value.HasFlag(this.Settings.ToggleSkillSetNode)) {
                        Input.KeyDown(this.Settings.HotKey);
                        Input.KeyUp(this.Settings.HotKey);

                        _lastCast = DateTime.UtcNow;
                    }
                    else if (!Input.IsKeyDown(this.Settings.ToggleSkillSetNode)) {
                        Input.KeyDown(this.Settings.HotKey);
                        Input.KeyUp(this.Settings.HotKey);

                        _lastCast = DateTime.UtcNow;
                    }
                }




            }
            return null;



        }
        #endregion

    }

}


