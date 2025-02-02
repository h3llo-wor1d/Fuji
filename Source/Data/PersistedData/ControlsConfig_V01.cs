using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Celeste64;

public class ControlsConfig_V01 : PersistedData
{
	public override int Version => 1;

	public Dictionary<string, List<ControlsConfigBinding>> Actions { get; set; } = [];
	public Dictionary<string, ControlsConfigStick> Sticks { get; set; } = [];

	public static ControlsConfig_V01 Defaults = new()
	{
		Actions = new()
		{
			["Jump"] = [
				new(Keys.C),
				new(Buttons.South),
				new(Buttons.North),
			],
			["Dash"] = [
				new(Keys.X),
				new(Buttons.West),
				new(Buttons.East),
			],
			["Climb"] = [
				new(Keys.Z),
				new(Keys.V),
				new(Keys.LeftShift),
				new(Keys.RightShift),
				new(Buttons.LeftShoulder),
				new(Buttons.RightShoulder),
				new(Axes.LeftTrigger, 0.4f, false),
				new(Axes.RightTrigger, 0.4f, false),
			],
			["Confirm"] = [
				new(Keys.C),
				new(Buttons.South) { NotFor = Gamepads.Nintendo },
				new(Buttons.East) { OnlyFor = Gamepads.Nintendo },
			],
			["Cancel"] = [
				new(Keys.X),
				new(Buttons.East) { NotFor = Gamepads.Nintendo },
				new(Buttons.South) { OnlyFor = Gamepads.Nintendo },
			],
			["Pause"] = [
				new(Keys.Escape),
				new(Keys.Enter),
				new(Buttons.Start),
				new(Buttons.Select),
				new(Buttons.Back)
			],
			["CopyFile"] = [
				new(Keys.V),
				new(Buttons.LeftShoulder)
			],
			["DeleteFile"] = [
				new(Keys.N),
				new(Buttons.RightShoulder)
			],
			["CreateFile"] = [
				new(Keys.B),
				new(Buttons.North)
			],
			["ResetBindings"] = [
				new(Keys.V),
				new(Buttons.LeftShoulder)
			],
			["ClearBindings"] = [
				new(Keys.B),
				new(Buttons.North)
			],
			["FullScreen"] = [
				new(Keys.F4),
			],
			["ReloadAssets"] = [
				new(Keys.F5),
			],
			["DebugMenu"] = [
				new(Keys.F6),
			],
			["Restart"] = [
				new(Keys.R),
			],
		},

		Sticks = new()
		{
			["Move"] = new()
			{
				Deadzone = 0.35f,
				Left = [
					new(Keys.Left),
					new(Buttons.Left),
					new(Axes.LeftX, 0.0f, true)
				],
				Right = [
					new(Keys.Right),
					new(Buttons.Right),
					new(Axes.LeftX, 0.0f, false)
				],
				Up = [
					new(Keys.Up),
					new(Buttons.Up),
					new(Axes.LeftY, 0.0f, true)
				],
				Down = [
					new(Keys.Down),
					new(Buttons.Down),
					new(Axes.LeftY, 0.0f, false)
				],
			},
			["Camera"] = new()
			{
				Deadzone = 0.35f,
				Left = [
					new(Keys.A),
					new(Axes.RightX, 0.0f, true)
				],
				Right = [
					new(Keys.D),
					new(Axes.RightX, 0.0f, false)
				],
				Up = [
					new(Keys.W),
					new(Axes.RightY, 0.0f, true)
				],
				Down = [
					new(Keys.S),
					new(Axes.RightY, 0.0f, false)
				],
			},
			["Menu"] = new()
			{
				Deadzone = 0.35f,
				Left = [
					new(Keys.Left),
					new(Buttons.Left),
					new(Axes.LeftX, 0.50f, true)
				],
				Right = [
					new(Keys.Right),
					new(Buttons.Right),
					new(Axes.LeftX, 0.50f, false)
				],
				Up = [
					new(Keys.Up),
					new(Buttons.Up),
					new(Axes.LeftY, 0.50f, true)
				],
				Down = [
					new(Keys.Down),
					new(Buttons.Down),
					new(Axes.LeftY, 0.50f, false)
				],
			},
		}
	};

	public override JsonTypeInfo GetTypeInfo()
	{
		return ControlsConfig_V01Context.Default.ControlsConfig_V01;
	}
}

[JsonSourceGenerationOptions(
	WriteIndented = true,
	UseStringEnumConverter = true,
	DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
	AllowTrailingCommas = true,
	Converters = [typeof(ControlsConfigBinding_Converter)]
)]
[JsonSerializable(typeof(ControlsConfig_V01))]
internal partial class ControlsConfig_V01Context : JsonSerializerContext { }
