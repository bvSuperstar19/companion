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

namespace Companion
{
    public class Companion : BaseSettingsPlugin<CompanionSetting>
    {
        private DateTime _lastCast = DateTime.MinValue;
        

        private void OnToggleSSkillSetNodeChange()
        {
            Input.RegisterKey(this.Settings.ToggleSkillSetNode);
        }

        public override bool Initialise()
        {
            Input.RegisterKey(this.Settings.ToggleSkillSetNode);
            this.Settings.ToggleSkillSetNode.OnValueChanged += OnToggleSSkillSetNodeChange;
            return true;
        }

        public override Job Tick()
        {
            var player = this.GameController.Game.IngameState.Data.LocalPlayer;
            var entities = this.GameController.Entities;
            var buffs = player.Buffs;
            var life = player.GetComponent<Life>();

           

            if (!this.Settings.Enable)
            {
                return null;
            }

            // Returns null if not enough mana
            if (this.Settings.MinimumMana > life.CurMana)
            {
                return null;
            }

            // Return null if cool down span
            TimeSpan cooldownSpan = TimeSpan.FromMilliseconds(this.Settings.CoolDown);
            if (_lastCast + cooldownSpan > DateTime.UtcNow)
            {
                return null;
            }

            bool enemiesNearby = false;
            foreach (var entity in entities)
            {
                if (entity.Rarity == ExileCore.Shared.Enums.MonsterRarity.White && this.Settings.WhiteMonster==false)
                {
                    break;
                }
                else if (entity.Rarity == ExileCore.Shared.Enums.MonsterRarity.Magic && this.Settings.MagiceMonster == false)
                {
                    break;
                }
                else if (entity.Rarity == ExileCore.Shared.Enums.MonsterRarity.Rare && this.Settings.RareMonster == false)
                {
                    break;
                }
                else if (entity.Rarity == ExileCore.Shared.Enums.MonsterRarity.Unique && this.Settings.UniqueMonster == false)
                {
                    break;
                }


                if (entity.IsAlive && entity.IsHostile && entity.IsTargetable)
                {
                    float distance = (player.Pos - entity.Pos).Length();
                    if (distance <= this.Settings.Range)
                    {
                        enemiesNearby = true;
                    }
                }
            }

            bool auraOn = buffs.Any(b => b.Name == "corrosive_shroud_aura");


            bool switchState = false;
            if (enemiesNearby)
            {
                // Turn aura on if enemies nearby
                if (!auraOn)
                {
                    switchState = true;
                }
            }
            else
            {
                // Turn off aura if enemies not nearby
                if (auraOn)
                {
                    switchState = true;
                }
            }

            if (switchState)
            {
                if (this.Settings.HotKey.Value.HasFlag(this.Settings.ToggleSkillSetNode))
                {
                    Input.KeyDown(this.Settings.HotKey);
                    Input.KeyUp(this.Settings.HotKey);

                    _lastCast = DateTime.UtcNow;
                }
                else if (!Input.IsKeyDown(this.Settings.ToggleSkillSetNode))
                {
                    Input.KeyDown(this.Settings.HotKey);
                    Input.KeyUp(this.Settings.HotKey);

                    _lastCast = DateTime.UtcNow;
                }
            }

            return null;
        }
    }
}
