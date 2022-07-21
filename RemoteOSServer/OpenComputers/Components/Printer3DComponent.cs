using System.Drawing;

namespace RemoteOS.OpenComputers.Components
{
    [Component("printer3d")]
    public class Printer3DComponent : Component
    {
        public Printer3DComponent(Machine parent, Guid address) : base(parent, address)
        {
        }

        int? _maxShapes;

        public async Task<bool> Print(int count = 1) => (await Invoke("commit", count))[0];
        public async Task AddShape(int minX, int minY, int minZ, int maxX, int maxY, int maxZ, string texture, Color tint) => await AddShape(minX, minY, minZ, maxX, maxY, maxZ, texture, false, tint);

        public async Task AddShape(int minX, int minY, int minZ, int maxX, int maxY, int maxZ, string texture, bool state = false) => await AddShape(minX, minY, minZ, maxX, maxY, maxZ, texture, state, Color.Transparent);

        public async Task AddShape(int minX, int minY, int minZ, int maxX, int maxY, int maxZ, string texture, bool state, Color tint)
        {
            if (minX == maxX || minY == maxY || minZ == maxZ) throw new ArgumentException("Empty block");
            if (
                minX < 0 || minX > 15 ||
                minY < 0 || minY > 15 ||
                minZ < 0 || minZ > 15 ||
                maxX < 0 || maxX > 15 ||
                maxY < 0 || maxY > 15 ||
                maxZ < 0 || maxZ > 15
            ) throw new ArgumentOutOfRangeException("Coordinates must be in range 0..15");
            await Invoke("addShape", minX, minY, minZ, maxX, maxY, maxZ, $"{texture}", state, tint);
        }
        public async Task Reset() => await Invoke("reset");
        public async Task<string> GetLabel() => (await Invoke("getLabel"))[0];
        public async Task SetLabel(string label) => await Invoke("setLabel", $@"""{label}""");
        public async Task<string> GetTooltip() => (await Invoke("getTooltip"))[0];
        public async Task SetTooltip(string tooltip) => await Invoke("setTooltip", $@"""{tooltip}""");
        public async Task<bool> IsButtonMode() => (await Invoke("isButtonMode"))[0];
        public async Task SetButtonMode(bool button) => await Invoke("setButtonMode", button);
        public async Task<int> GetRedstoneLevel() => (await Invoke("isRedstoneEmitter"))[1];
        public async Task SetRedstoneLevel(int level)
        {
            if (level < 0 || level > 15) throw new ArgumentOutOfRangeException("Redstone level cannot be less than 0 or higher than 15");
            await Invoke("setRedstoneEmitter", level);
        }
        public async Task<(bool Default, bool Active)> GetCollideMode()
        {
            var r = await Invoke("isCollidable");
            return (r[0], r[1]);
        }
        public async Task SetCollideMode(bool def, bool active) => await Invoke("setCollidable", def, active);
        public async Task<int> GetLightLevel() => (await Invoke("getLightLievel"))[0];
        public async Task SetLightLevel(int level) => await Invoke("setLightLevel", level);
        public async Task<int> GetShapeCount() => (await Invoke("getShapeCount"))[0];
        public async Task<int> GetMaxShapeCount() => _maxShapes ??= (await Invoke("getMaxShapeCount"))[0];
        public async Task<(bool CanPrint, string Status, double Progress)> GetStatus()
        {
            var res = await Invoke("status");
            var isb = res[1].IsBoolean;
            return (isb ? res[1] : false, res[0], isb ? 0 : res[1]);
        }

#if ROS_PROPERTIES
#if ROS_PROPS_UNCACHED
        public string Label
        {
            get => GetLabel().Result;
            set => SetLabel(value);
        }

        public string ToolTip
        {
            get => GetTooltip().Result;
            set => SetTooltip(value);
        }

        public bool ButtonMode
        {
            get => IsButtonMode().Result;
            set => SetButtonMode(value);
        }

        public int RedstoneLevel
        {
            get => GetRedstoneLevel().Result;
            set => SetRedstoneLevel(value);
        }

        public (bool Default, bool Active) CollideMode
        {
            get => GetCollideMode().Result;
            set => SetCollideMode(value.Default, value.Active);
        }

        public int LightLevel
        {
            get => GetLightLevel().Result;
            set => SetLightLevel(value);
        }
        public int ShapeCount => GetShapeCount().Result;

        public (bool CanPrint, string Status, double Progress) Status => GetStatus().Result;
#endif

        public int MaxShapeCount => GetMaxShapeCount().Result;
#endif
    }
}
