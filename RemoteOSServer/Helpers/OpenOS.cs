#if OpenOS
#define Agent
#define Robot
#define Computer

using OneOf;
using OneOf.Types;
using RemoteOS.OpenComputers.Components;
using RemoteOS.OpenComputers.Data;

namespace RemoteOS.Helpers
{
    public static class OpenOS
    {
        #region Agent
#if Agent
        public static Task<(bool Passable, string Description)> Detect(this Agent robot) => robot.Detect(Sides.Front);
        public static Task<(bool Passable, string Description)> DetectUp(this Agent robot) => robot.Detect(Sides.Top);
        public static Task<(bool Passable, string Description)> DetectDown(this Agent robot) => robot.Detect(Sides.Bottom);
        public static Task<bool> Compare(this Agent robot) => robot.Compare(Sides.Front);
        public static Task<bool> CompareUp(this Agent robot) => robot.Compare(Sides.Top);
        public static Task<bool> CompareDown(this Agent robot) => robot.Compare(Sides.Bottom);
        public static Task<bool> Drop(this Agent robot) => robot.Drop(Sides.Front);
        public static Task<bool> DropUp(this Agent robot) => robot.Drop(Sides.Top);
        public static Task<bool> DropDown(this Agent robot) => robot.Drop(Sides.Bottom);
        public static Task<OneOf<int, Error>> Suck(this Agent robot, int amount) => robot.Suck(Sides.Front, amount);
        public static Task<OneOf<int, Error>> SuckUp(this Agent robot, int amount) => robot.Suck(Sides.Top, amount);
        public static Task<OneOf<int, Error>> SuckDown(this Agent robot, int amount) => robot.Suck(Sides.Bottom, amount);
        public static Task<ReasonOr<Success>> PlaceUp(this Agent robot, Sides side = Sides.Top, bool sneaky = false) => robot.Place(side, sneaky);
        public static Task<ReasonOr<Success>> PlaceDown(this Agent robot, Sides side = Sides.Bottom, bool sneaky = false) => robot.Place(side, sneaky);
        public static Task<ReasonOr<Success>> SwingUp(this Agent robot, Sides side = Sides.Top, bool sneaky = false) => robot.Swing(side, sneaky);
        public static Task<ReasonOr<Success>> SwingDown(this Agent robot, Sides side = Sides.Bottom, bool sneaky = false) => robot.Swing(side, sneaky);
        public static Task<ReasonOr<Success>> UseUp(this Agent robot, Sides side = Sides.Top, bool sneaky = false, int duration = 0) => robot.Use(side, sneaky, duration);
        public static Task<ReasonOr<Success>> UseDown(this Agent robot, Sides side = Sides.Bottom, bool sneaky = false, int duration = 0) => robot.Use(side, sneaky, duration);
        public static Task<bool> CompareFluid(this Agent robot) => robot.CompareFluid(Sides.Front);
        public static Task<bool> CompareFluidUp(this Agent robot) => robot.CompareFluid(Sides.Top);
        public static Task<bool> CompareFluidDown(this Agent robot) => robot.CompareFluid(Sides.Bottom);
        public static Task<OneOf<int, Error>> Drain(this Agent robot, int amount = 1000) => robot.Drain(Sides.Front, amount);
        public static Task<OneOf<int, Error>> DrainUp(this Agent robot, int amount = 1000) => robot.Drain(Sides.Top, amount);
        public static Task<OneOf<int, Error>> DrainDown(this Agent robot, int amount = 1000) => robot.Drain(Sides.Bottom, amount);
        public static Task<OneOf<int, Error>> Fill(this Agent robot, int amount = 1000) => robot.Fill(Sides.Front, amount);
        public static Task<OneOf<int, Error>> FillUp(this Agent robot, int amount = 1000) => robot.Fill(Sides.Top, amount);
        public static Task<OneOf<int, Error>> FillDown(this Agent robot, int amount = 1000) => robot.Fill(Sides.Bottom, amount);
#endif
        #endregion
        #region Robot
#if Robot
        public static Task<ReasonOr<Success>> Forward(this RobotComponent robot) => robot.Move(Sides.Front);
        public static Task<ReasonOr<Success>> Back(this RobotComponent robot) => robot.Move(Sides.Back);
        public static Task<ReasonOr<Success>> Up(this RobotComponent robot) => robot.Move(Sides.Top);
        public static Task<ReasonOr<Success>> Down(this RobotComponent robot) => robot.Move(Sides.Bottom);
        public static Task<bool> TurnLeft(this RobotComponent robot) => robot.Turn(false);
        public static Task<bool> TurnRight(this RobotComponent robot) => robot.Turn(true);
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
