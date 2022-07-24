#if OpenOS
#define Agent
#define Robot
#define Computer

using RemoteOS.OpenComputers.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteOS.OpenComputers
{
    public static class OpenOS
    {
        #region Agent
#if Agent
        public static async Task<(bool Passable, string Description)> Detect(this Agent robot) => await robot.Detect(Sides.Front);
        public static async Task<(bool Passable, string Description)> DetectUp(this Agent robot) => await robot.Detect(Sides.Top);
        public static async Task<(bool Passable, string Description)> DetectDown(this Agent robot) => await robot.Detect(Sides.Bottom);
        public static async Task<bool> Compare(this Agent robot) => await robot.Compare(Sides.Front);
        public static async Task<bool> CompareUp(this Agent robot) => await robot.Compare(Sides.Top);
        public static async Task<bool> CompareDown(this Agent robot) => await robot.Compare(Sides.Bottom);
        public static async Task<bool> Drop(this Agent robot) => await robot.Drop(Sides.Front);
        public static async Task<bool> DropUp(this Agent robot) => await robot.Drop(Sides.Top);
        public static async Task<bool> DropDown(this Agent robot) => await robot.Drop(Sides.Bottom);
        public static async Task<(bool Success, int Amount)> Suck(this Agent robot, int amount) => await robot.Suck(Sides.Front, amount);
        public static async Task<(bool Success, int Amount)> SuckUp(this Agent robot, int amount) => await robot.Suck(Sides.Top, amount);
        public static async Task<(bool Success, int Amount)> SuckDown(this Agent robot, int amount) => await robot.Suck(Sides.Bottom, amount);
        public static async Task<(bool Success, string Reason)> PlaceUp(this Agent robot, Sides side = Sides.Top, bool sneaky = false) => await robot.Place(side, sneaky);
        public static async Task<(bool Success, string Reason)> PlaceDown(this Agent robot, Sides side = Sides.Bottom, bool sneaky = false) => await robot.Place(side, sneaky);
        public static async Task<(bool Success, string Reason)> SwingUp(this Agent robot, Sides side = Sides.Top, bool sneaky = false) => await robot.Swing(side, sneaky);
        public static async Task<(bool Success, string Reason)> SwingDown(this Agent robot, Sides side = Sides.Bottom, bool sneaky = false) => await robot.Swing(side, sneaky);
        public static async Task<(bool Success, string Reason)> UseUp(this Agent robot, Sides side = Sides.Top, bool sneaky = false, int duration = 0) => await robot.Use(side, sneaky, duration);
        public static async Task<(bool Success, string Reason)> UseDown(this Agent robot, Sides side = Sides.Bottom, bool sneaky = false, int duration = 0) => await robot.Use(side, sneaky, duration);
        public static async Task<bool> CompareFluid(this Agent robot) => await robot.CompareFluid(Sides.Front);
        public static async Task<bool> CompareFluidUp(this Agent robot) => await robot.CompareFluid(Sides.Top);
        public static async Task<bool> CompareFluidDown(this Agent robot) => await robot.CompareFluid(Sides.Bottom);
        public static async Task<(bool Success, int Amount)> Drain(this Agent robot, int amount = 1000) => await robot.Drain(Sides.Front, amount);
        public static async Task<(bool Success, int Amount)> DrainUp(this Agent robot, int amount = 1000) => await robot.Drain(Sides.Top, amount);
        public static async Task<(bool Success, int Amount)> DrainDown(this Agent robot, int amount = 1000) => await robot.Drain(Sides.Bottom, amount);
        public static async Task<(bool Success, int Amount)> Fill(this Agent robot, int amount = 1000) => await robot.Fill(Sides.Front, amount);
        public static async Task<(bool Success, int Amount)> FillUp(this Agent robot, int amount = 1000) => await robot.Fill(Sides.Top, amount);
        public static async Task<(bool Success, int Amount)> FillDown(this Agent robot, int amount = 1000) => await robot.Fill(Sides.Bottom, amount);
#endif
        #endregion
        #region Robot
#if Robot
        public static async Task<(bool Success, string Reason)> Forward(this RobotComponent robot) => await robot.Move(Sides.Front);
        public static async Task<(bool Success, string Reason)> Back(this RobotComponent robot) => await robot.Move(Sides.Back);
        public static async Task<(bool Success, string Reason)> Up(this RobotComponent robot) => await robot.Move(Sides.Top);
        public static async Task<(bool Success, string Reason)> Down(this RobotComponent robot) => await robot.Move(Sides.Bottom);
        public static async Task<bool> TurnLeft(this RobotComponent robot) => await robot.Turn(false);
        public static async Task<bool> TurnRight(this RobotComponent robot) => await robot.Turn(true);
        public static async Task TurnAround(this RobotComponent robot)
        {
            await robot.TurnRight();
            await robot.TurnRight();
        }

#endif
#endregion
        #region Computer
#if Computer
        public static bool IsRobot(this ComputerComponent computer) => computer.Parent.Components.IsAvailable<RobotComponent>();


#endif
        #endregion
    }
}
#endif
