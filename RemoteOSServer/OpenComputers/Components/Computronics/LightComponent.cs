using OneOf.Types;
using RemoteOS.Helpers;
using System.Drawing;

// ReSharper disable ClassNeverInstantiated.Global

namespace RemoteOS.OpenComputers.Components.Computronics
{
    [Component("light_board")]
    public partial class LightComponent : Component
    {
        private Color?[]? _colorCache;
        private bool?[]? _activeCache;

        public LightComponent(Machine parent, Guid address) : base(parent, address)
        {
        }

        /// <summary>
        /// Sets the color of the specified light.
        /// </summary>
        /// <param name="index">Light index</param>
        /// <param name="color">The new color</param>
        /// <returns>Whether the color was changed successfully, and the reason why if it wasnt</returns>
        /// <exception cref="IndexOutOfRangeException">This light does not exist</exception>
        public async Task<ReasonOr<Success>> SetColor(int index, Color color)
        {
            if (index < 0 || index >= await GetLightCount()) throw new IndexOutOfRangeException("This light does not exist");
            var res = await GetInvoker()(index + 1, color);
            if (res[0])
            {
                _colorCache![index] = color;
                return new Success();
            }
            return res[1].Value;
        }

        /// <param name="index">Light index</param>
        /// <returns>The color of the specified light on success, an error message otherwise</returns>
        /// <exception cref="IndexOutOfRangeException">This light does not exist</exception>
        public async Task<ReasonOr<Color>> GetColor(int index)
        {
            if (index < 0 || index >= await GetLightCount()) throw new IndexOutOfRangeException("This light does not exist");
            if (_colorCache![index].HasValue) return _colorCache![index]!;
            var res = await GetInvoker()(index + 1);
            if (res[0].IsBoolean) return res[1].Value;
            var color = Color.FromArgb(res[0]);
            _colorCache![index] = color;
            return color;
        }

        /// <summary>
        /// Turns the specified light on or off.
        /// </summary>
        /// <param name="index">Light index</param>
        /// <param name="state">The new state</param>
        /// <returns>Whether the state change was successful, and the reason why if it wasnt</returns>
        /// <exception cref="IndexOutOfRangeException">This light does not exist</exception>
        public async Task<ReasonOr<Success>> SetActive(int index, bool state)
        {
            if (index < 0 || index >= await GetLightCount()) throw new IndexOutOfRangeException("This light does not exist");
            var res = await GetInvoker()(index + 1, state);
            if (!res[0]) return res[1].Value;
            _activeCache![index] = state;
            return new Success();
        }

        /// <param name="index">Light index</param>
        /// <returns>True if the light at the specified position is currently active</returns>
        /// <exception cref="IndexOutOfRangeException">This light does not exist</exception>
        public async Task<bool> IsActive(int index)
        {
            if (index < 0 || index >= await GetLightCount()) throw new IndexOutOfRangeException("This light does not exist");
            if (_activeCache![index].HasValue) return _activeCache![index]!.Value;
            var res = (await GetInvoker()(index + 1))[0];
            return (_activeCache![index] = res).Value;
        }

        /// <returns>The number of lights on the board.</returns>
        public async Task<int> GetLightCount()
        {
            if(_colorCache is null || _activeCache is null || _colorCache.Length != _activeCache.Length)
            {
                var res = (await GetInvoker()())[0];
                _colorCache = new Color?[res];
                _activeCache = new bool?[res];
                return res;
            }
            return _colorCache.Length;
        }
    }
}