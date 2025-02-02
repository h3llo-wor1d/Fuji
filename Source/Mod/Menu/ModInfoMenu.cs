namespace Celeste64.Mod;

public class ModInfoMenu : Menu
{
	public Target Target;

	Subtexture postcardImage;
	Subtexture stampImage;
	Subtexture strawberryImage;

	internal GameMod? Mod;

	public Menu? depWarningMenu;
	public Menu? safeDisableErrorMenu;
	public ModOptionsMenu modOptionsMenu;

	internal ModInfoMenu(Menu? rootMenu)
	{
		Target = new Target(Overworld.CardWidth, Overworld.CardHeight);
		Game.OnResolutionChanged += () => Target = new Target(Overworld.CardWidth, Overworld.CardHeight);

		RootMenu = rootMenu;
		postcardImage = new(Assets.Textures["postcards/back-empty"]);
		stampImage = Assets.Subtextures["stamp"];
		strawberryImage = Assets.Subtextures["icon_strawberry"];

		modOptionsMenu = new ModOptionsMenu(rootMenu);

		InitItems();
	}

	private void InitItems()
	{
		items.Clear();
		Add(new Toggle(
			"ModEnabled",
			() =>
			{
				//If we are trying to disable the current mod, don't
				if (Mod != null && Mod != ModManager.Instance.CurrentLevelMod)
				{
					var modSettings = ModSettings.GetOrMakeModSettings(Mod.ModInfo.Id);
					modSettings.Enabled = !modSettings.Enabled;
					Mod.NeedsReload = true;

					if (modSettings.Enabled)
					{
						Mod.EnableDependencies(); // Also enable dependencies of the mod being enabled (if any).
						Mod.SetNeedsReloadRecursive();
					}
					else
					{
						if (Mod.DisableSafe(true)) // If it is not safe to disable the mod
						{
							safeDisableErrorMenu = new Menu { Title = Loc.Str("ModSafeDisableErrorMessage") };
							safeDisableErrorMenu.Add(new Option("Exit", () =>
							{
								modSettings.Enabled = true; // Override the toggle if the operation can't be done.

								PopRootSubMenu();
							}));

							PushRootSubMenu(safeDisableErrorMenu);

							return;
						}

						if (Mod.GetDependents().Count > 0)
						{
							depWarningMenu = new Menu { Title = $"Warning, this mod is depended on by {Mod.GetDependents().Count} other mod(s).\nIf you disable this mod, those mods will also be disabled." };

							depWarningMenu.Add(new Option("ConfirmDisableMod", () =>
							{
								Mod.DisableSafe(false);

								RootMenu?.PopSubMenu();
							}));
							depWarningMenu.Add(new Option("Exit", () =>
							{
								modSettings.Enabled = true; // Override the toggle if the operation was cancelled.

								RootMenu?.PopSubMenu();
							}));

							RootMenu?.PushSubMenu(depWarningMenu);
						}
					}
				}
				else
				{
					safeDisableErrorMenu = new Menu { Title = Loc.Str("ModSafeDisableErrorMessage") };

					safeDisableErrorMenu.Add(new Option("Exit", () =>
					{
						if (Mod != null)
						{
							ModSettings.GetOrMakeModSettings(Mod.ModInfo.Id).Enabled = true; // Override the toggle if the operation can't be done.
						}

						RootMenu?.PopSubMenu();
					}));

					RootMenu?.PushSubMenu(safeDisableErrorMenu);
				}
			},
			() => Mod != null ? ModSettings.GetOrMakeModSettings(Mod.ModInfo.Id).Enabled : false)
		);
		if (modOptionsMenu.ShouldDisplay)
		{
			Add(new Submenu("ModOptions", RootMenu, modOptionsMenu));
		}
		Add(new Option("Back", () =>
		{
			if (RootMenu != null)
			{
				RootMenu.PopSubMenu();
			}
		}));
	}

	internal void SetMod(GameMod mod)
	{
		Mod = mod;
		modOptionsMenu.SetMod(mod);

		InitItems();
	}

	public override void Initialized()
	{
		base.Initialized();
	}

	protected override void RenderItems(Batcher batch)
	{
		var font = Language.Current.SpriteFont;

		if (Mod != null && postcardImage.Texture != null)
		{
			// TODO: This is messy and probably should be cleaned up and rewritten, but I'm holding off for now because I think we might want to do a visual polish pass on it later anyways.
			var bounds = Target.Bounds;
			var scale = MathF.Max(bounds.Width / postcardImage.Width, bounds.Height / postcardImage.Height);
			var size = new Vec2(postcardImage.Width, postcardImage.Height);
			batch.Image(postcardImage, bounds.TopLeft, size / 2, Vec2.One * scale, 0, Color.White);
			batch.Text(font, $"{Mod.ModInfo.Name}\nBy: {Mod.ModInfo.ModAuthor ?? "Unknown"}\nv.{Mod.ModInfo.Version}", bounds.TopLeft + (-size / 4 + new Vec2(16, 12)) * Game.RelativeScale, Color.Black * 0.7f);
			batch.PushMatrix(Matrix3x2.CreateScale(.7f) * Matrix3x2.CreateTranslation(bounds.TopLeft + (-size / 4 + new Vec2(14, 72)) * Game.RelativeScale));
			batch.Text(font, GenerateModDescription(Mod.ModInfo.Description ?? "", 40, 15), Vec2.Zero, Color.Black);
			batch.PopMatrix();

			float imgScale = 0.9f;
			var image = Mod.Subtextures.TryGetValue(Mod.ModInfo.Icon ?? "", out var value) ? value : strawberryImage;
			float imgSizeMin = MathF.Min(postcardImage.Width, postcardImage.Height) / 6;
			var stampImageSize = new Vec2(imgSizeMin / stampImage.Width, imgSizeMin / stampImage.Height);
			var imageSize = new Vec2(imgSizeMin / image.Width, imgSizeMin / image.Height);
			var stampPos = bounds.TopLeft - (new Vec2(imgSizeMin, imgSizeMin) * imgScale) / 2 + new Vec2(size.X / 5.5f, -size.Y / 4.7f);
			var pos = bounds.TopLeft - (new Vec2(imgSizeMin, imgSizeMin) * imgScale) / 2 + new Vec2(size.X / 5.05f, -size.Y / 5.3f);
			batch.Image(stampImage, (stampPos + new Vec2(imgSizeMin, imgSizeMin) * imgScale * 0.05f) * Game.RelativeScale, stampImageSize * imgScale * Game.RelativeScale, stampImageSize * imgScale * 1.3f * Game.RelativeScale, 0, Color.White);
			batch.Image(image, (pos + new Vec2(imgSizeMin, imgSizeMin) * imgScale * 0.05f) * Game.RelativeScale, imageSize * imgScale * Game.RelativeScale, imageSize * imgScale * Game.RelativeScale, 0, Color.White);

			batch.PushMatrix(Matrix3x2.CreateScale(1.0f) * Matrix3x2.CreateTranslation(bounds.TopLeft + new Vec2(size.X / 6.8f, -size.Y / 20) * Game.RelativeScale));
			base.RenderItems(batch);
			batch.PopMatrix();
		}
	}

	private static string GenerateModDescription(string str, int maxLength, int maxLines)
	{
		List<string> words = str.Split(' ').ToList();
		List<string> lines = [""];
		for (int wordIndex = 0; wordIndex < words.Count;)
		{
			if (lines.Count > maxLines) break;
			string word = words[wordIndex];
			if ((lines[lines.Count - 1] + word).Length <= maxLength)
			{
				lines[lines.Count - 1] = lines[lines.Count - 1] + " " + word;
				wordIndex++;
				continue;
			}
			else if (lines.Last().Length == 0)
			{
				lines[lines.Count - 1] = word.Substring(0, maxLength);
				words.Insert(wordIndex + 1, word.Substring(maxLength));
				lines.Add("");
				wordIndex++;
				continue;
			}
			else
			{
				lines.Add("");
			}
		}

		return string.Join("\n", lines);
	}
}
