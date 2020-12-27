using ExileCore.Shared.Interfaces;
using ExileCore.Shared.Nodes;
using System.Windows.Forms;

namespace Companion
{
    public class CompanionSetting : ISettings
    {
        public ToggleNode Enable { get; set; } = new ToggleNode(true);


        public RangeNode<int> MinimumMana { get; set; } = new RangeNode<int>(6, 0, 100);
        public RangeNode<int> Range { get; set; } = new RangeNode<int>(500, 0, 10000);
        public RangeNode<int> CoolDown { get; set; } = new RangeNode<int>(511, 0, 25000);

        public ToggleNode WhiteMonster { get; set; } = new ToggleNode(true);
        public ToggleNode MagiceMonster { get; set; } = new ToggleNode(true);
        public ToggleNode RareMonster { get; set; } = new ToggleNode(true);
        public ToggleNode UniqueMonster { get; set; } = new ToggleNode(true);

        public HotkeyNode HotKey { get; set; } = new HotkeyNode(Keys.None);
        public HotkeyNode ToggleSkillSetNode { get; set; } = new HotkeyNode(Keys.ControlKey);
    }
}
