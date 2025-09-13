using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Framework.ModLoading.Rewriters.StardewValley_1_6;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using StardewValley.Menus;

namespace NextFestivalReminder
{
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            helper.Events.Display.RenderedHud += this.OnRenderedHud;
        }

        private FestivalInfo nextFestival;
        private string lastSeason;
        private int lastDay;

        private FestivalInfo GetNextFestival()
        {
            if (nextFestival != null && lastSeason == Game1.currentSeason && lastDay == Game1.dayOfMonth)
                return nextFestival;

            lastSeason = Game1.currentSeason;
            lastDay = Game1.dayOfMonth;

            Dictionary<string, string> festivals = DataLoader.Festivals_FestivalDates(Game1.temporaryContent);
            string currentSeason = lastSeason.ToLower();
            int currentDay = lastDay;

            foreach (var kv in festivals.OrderBy(kv => kv.Key))
            {
                string season = new string(kv.Key.TakeWhile(c => !char.IsDigit(c)).ToArray());
                int day = int.Parse(new string(kv.Key.SkipWhile(c => !char.IsDigit(c)).ToArray()));

                if (season == currentSeason && day > currentDay)
                {
                    nextFestival = new FestivalInfo(kv.Value, day, season);
                    break;
                }
                else if (string.Compare(season, currentSeason) > 0)
                {
                    nextFestival = new FestivalInfo(kv.Value, day, season);
                    break;
                }

            }

            return nextFestival;

        }

        private void OnRenderedHud(object sender, RenderedHudEventArgs e)
        {
            FestivalInfo festival = GetNextFestival();
            string text = festival.ToString();
            if (festival != null)
            {
                var spriteBatch = e.SpriteBatch;

                Vector2 textSize = Game1.smallFont.MeasureString(text);
                int paddingX = 8;
                int paddingY = 4;
                int width = (int)textSize.X + 2 * paddingX;
                int height = (int)textSize.Y + 2 * paddingY;

                float screenWidth = Game1.uiViewport.Width;
                float screenHeight = Game1.uiViewport.Height;

                float goldX = screenWidth - 128;
                float goldY = screenHeight * 0.02f;

                float offsetX = width * 0.1f;
                float offsetY = height * 3.5f;
                float x = goldX - width + offsetX + screenWidth * 0.08f;
                float y = goldY + offsetY;

                Color hudOrange = new Color(0xDC, 0x7B, 0x05);

                Texture2D whitePixel = Game1.fadeToBlackRect;
                Texture2D wizardFurniture = Game1.content.Load<Texture2D>("TileSheets/wizard_furniture");

                int iconSize = 32;
                Rectangle sourceRect = new Rectangle(147, 240, 10, 13);
                spriteBatch.Draw(wizardFurniture, new Rectangle((int)x, (int)y, iconSize, iconSize), sourceRect, hudOrange);

                Rectangle iconRect = new Rectangle((int)x, (int)y, iconSize, iconSize);
                if (iconRect.Contains(Game1.getMouseX(), Game1.getMouseY())) {
                    IClickableMenu.drawHoverText(spriteBatch, festival.ToString(), Game1.smallFont);
                }
            }

        }
    }
}
