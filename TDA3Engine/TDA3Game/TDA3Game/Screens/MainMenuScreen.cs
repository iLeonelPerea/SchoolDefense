﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScreenSystemLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TDA3Engine;

namespace TDA3Game
{
    public class MainMenuScreen : MenuScreen
    {

        MainMenuEntry play, options, help, quit;
         MapLoader ml;
         Map selectedMap;

        string prevEntry, nextEntry, selectedEntry, cancelMenu;
        public override string PreviousEntryActionName
        {
            get { return prevEntry; }
        }

        public override string NextEntryActionName
        {
            get { return nextEntry; }
        }

        public override string SelectedEntryActionName
        {
            get { return selectedEntry; }
        }

        public override string MenuCancelActionName
        {
            get { return cancelMenu; }
        }

        public MainMenuScreen()
        {
            prevEntry = "MenuUp";
            nextEntry = "MenuDown";
            selectedEntry = "MenuAccept";
            cancelMenu = "MenuCancel";

            TransitionOnTime = TimeSpan.FromSeconds(1);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            Selected = Highlighted = new Color(214, 232, 223);
            Normal = new Color(104, 173, 178);
        }


        public override void InitializeScreen()
        {
            InputMap.NewAction(PreviousEntryActionName, Keys.Up);
            InputMap.NewAction(NextEntryActionName, Keys.Down);
            InputMap.NewAction(SelectedEntryActionName, Keys.Enter);
            InputMap.NewAction(SelectedEntryActionName, MousePresses.LeftMouse);
            InputMap.NewAction(MenuCancelActionName, Keys.Escape);

            play = new MainMenuEntry(this, "Play", "PLAY GAME");
            options = new MainMenuEntry(this, "Options", "CHANGE GAME SETTINGS");
            help = new MainMenuEntry(this, "Help", "INPUT DIAGRAM AND GENERAL GAME INFORMATION");
            quit = new MainMenuEntry(this, "Quit", "DONE PLAYING FOR NOW?");

            Removing += new EventHandler(MainMenuRemoving);
            Entering += new TransitionEventHandler(MainMenuScreen_Entering);
            Exiting += new TransitionEventHandler(MainMenuScreen_Exiting);

            play.Selected += new EventHandler(PlayNowSelect);
            options.Selected += new EventHandler(PlaySelect);
            help.Selected += new EventHandler(HelpSelect);
            quit.Selected += new EventHandler(QuitSelect);

            MenuEntries.Add(play);
            MenuEntries.Add(options);
            MenuEntries.Add(help);
            MenuEntries.Add(quit);

            Viewport view = ScreenSystem.Viewport;
            SetDescriptionArea(new Rectangle(100, view.Height - 100,
                view.Width - 100, 50), new Color(11, 38, 40), new Color(29, 108, 117),
                new Point(10, 0), 0.5f);

            //AudioManager.singleton.PlaySong("Menu");

        }

        public override void LoadContent()
        {

            ContentManager content = ScreenSystem.Content;
            SpriteFont = content.Load<SpriteFont>(@"Fonts/menu");

            Texture2D entryTexture =
                content.Load<Texture2D>(@"Textures/Menu/MenuEntries");

            BackgroundTexture = content.Load<Texture2D>(@"Textures/Menu/MainMenuBackground");

            //Loads the title texture.
            TitleTexture = content.Load<Texture2D>(@"Textures/Menu/LogoWithText");

            //Enable Mouse
            EnableMouse(content.Load<Texture2D>(@"Textures/Menu/mouse"));

            //Sets the title position
            InitialTitlePosition = TitlePosition =
                new Vector2((ScreenSystem.Viewport.Width - TitleTexture.Width) / 2, 50);

            //Initialize is called before LoadContent, so if you want to 
            //use relative position with the line spacing like below,
            //you need to do it after load content and spritefont
            for (int i = 0; i < MenuEntries.Count; i++)
            {
                MenuEntries[i].AddTexture(entryTexture, 2, 1,
                    new Rectangle(0, 0, entryTexture.Width / 2, entryTexture.Height),
                    new Rectangle(entryTexture.Width / 2, 0, entryTexture.Width / 2, entryTexture.Height));
                MenuEntries[i].AddPadding(14, 0);

                if (i == 0)
                    MenuEntries[i].SetPosition(new Vector2(180, 250), true);
                else
                {
                    int offsetY = MenuEntries[i - 1].EntryTexture == null ? SpriteFont.LineSpacing
                        : 8;
                    MenuEntries[i].SetRelativePosition(new Vector2(0, offsetY), MenuEntries[i - 1], true);
                }
                ml = ScreenSystem.Content.Load<MapLoader>("Maps\\MapLoader");
                selectedMap = ml.Maps[0];
            }
        }

        public override void UnloadContent()
        {
            SpriteFont = null;
        }

        void MainMenuScreen_Entering(object sender, TransitionEventArgs tea)
        {
            float effect = (float)Math.Pow(tea.percent - 1, 2) * -100;
            foreach (MenuEntry entry in MenuEntries)
            {
                entry.Acceleration = new Vector2(effect, 0);
                entry.Position = entry.DefaultPosition + entry.Acceleration;
                entry.Opacity = tea.percent;
            }

            TitlePosition = InitialTitlePosition + new Vector2(0, effect);
            TitleOpacity = tea.percent;
        }

        void MainMenuScreen_Exiting(object sender, TransitionEventArgs tea)
        {
            float effect = (float)Math.Pow(tea.percent - 1, 2) * 100;
            foreach (MenuEntry entry in MenuEntries)
            {
                entry.Acceleration = new Vector2(effect, 0);
                entry.Position = entry.DefaultPosition + entry.Acceleration;
                entry.Scale = tea.percent;
                entry.Opacity = tea.percent;
            }

            TitlePosition = InitialTitlePosition - new Vector2(0, effect);
            TitleOpacity = tea.percent;
        }

        void PlaySelect(object sender, EventArgs e)
        {
            ExitScreen();
            //ScreenSystem.AddScreen(new PlayScreen());
            ScreenSystem.AddScreen(new LevelSelectionScreen());
        }

        void PlayNowSelect(object sender, EventArgs e)
         {
             ExitScreen();
             ScreenSystem.AddScreen(new PlayScreen(new LevelSelectionScreen(), selectedMap));
         }

        void OptionsSelect(object sender, EventArgs e)
        {
            FreezeScreen();
            ScreenSystem.AddScreen(new OptionsScreen(this));
        }

        void HelpSelect(object sender, EventArgs e)
        {
            FreezeScreen();
            ScreenSystem.AddScreen(new HelpScreen(this));
        }

        void QuitSelect(object sender, EventArgs e)
        {
            ScreenSystem.Game.Exit();
        }


        void MainMenuRemoving(object sender, EventArgs e)
        {
            MenuEntries.Clear();
        }
    }
}
