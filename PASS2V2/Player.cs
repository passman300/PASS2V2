﻿using Animation2D;
using GameUtility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;

namespace PASS2V2
{
    public class Player
    {
        // player upgrades
        public const int NUM_UPGRADES = 4;
        public const int DOUBLE_SPEED_INDEX = 0;
        public const int TRIPLE_DAMAGE_INDEX = 1;
        public const int DOUBLE_FIRE_RATE_INDEX = 2;
        public const int DOUBLE_POINTS_INDEX = 3;

        // player dimensions
        public const int WIDTH = 64;
        public const int HEIGHT = 64;

        // player y location
        public const int Y_LOC = Game1.SCREEN_HEIGHT - HEIGHT;

        // player base speed
        public const int BASE_SPEED = 3;
        
        // player shooting speed
        public const float BASE_SHOOTING_DUR = 1000f / 3; // 3 shots per second

        public const int BASE_DAMAGE = 1;

        // local spritebatch
        private SpriteBatch spriteBatch;

        private Texture2D skin;

        private Vector2 loc;
        private Rectangle rec;

        private int speed;

        private Timer shootTimer = new Timer(BASE_SHOOTING_DUR, true);

        private bool isShoot = false;
        private Vector2 shootLocOffset = new Vector2(WIDTH / 2, 0);

        // player score
        private int highScore = 0;
        private int score = 0;

        // player damage
        private int damage = BASE_DAMAGE;

        // list of upgrades
        private bool[] buffs = new bool[NUM_UPGRADES];

        public bool IsShoot
        {
            get { return isShoot; }
            set { isShoot = value; }
        }

        public Vector2 Location
        {
            get { return loc; }
            set { loc = value; }
        }

        public Rectangle Rectangle
        {
            get { return rec; }
            set { rec = value; }
        }

        public Vector2 ShootLocOffset
        {
            get { return shootLocOffset; }
        }

        public int Score
        {
            get { return score; }
            set 
            { 
                int delta = value - score;
                // check if player has double points
                if (buffs[DOUBLE_POINTS_INDEX] && delta > 0) delta *= 2;
                score = Math.Max(0, score + delta);
            }
        }

        public int Damage
        {
            get 
            {
                if (buffs[TRIPLE_DAMAGE_INDEX]) return damage * 3;
                return damage;
            }
        }

        public bool[] UsedBuffs
        {
            get { return buffs; }
        }

        public int HighScore
        {
            get { return highScore; }
            set { highScore = value; }
        }

        public Player(SpriteBatch spriteBatch)
        {
            this.spriteBatch = spriteBatch;

            skin = Assets.alexImg;

            loc = new Vector2(Game1.SCREEN_WIDTH / 2 - WIDTH / 2, Y_LOC);
            rec = new Rectangle((int)loc.X, (int)loc.Y, WIDTH, HEIGHT);

            speed = BASE_SPEED;
        }


        public void Update(GameTime gameTime)
        {
            // update player position
            UpdateLocation(gameTime);

            // update player shooting
            UpdateShoot(gameTime);
        }

        private void UpdateShoot(GameTime gameTime)
        {
            // update shooting timer
            shootTimer.Update(gameTime);

            if (shootTimer.IsActive()) isShoot = false;
            else if (shootTimer.IsFinished() && Game1.kb.IsKeyDown(Keys.Space) && !Game1.prevKb.IsKeyDown(Keys.Space))
            {
                isShoot = true;

                // check if any shooting buffs
                if (buffs[DOUBLE_FIRE_RATE_INDEX])
                {
                    shootTimer.SetTargetTime(BASE_SHOOTING_DUR / 2);
                }
                else shootTimer.SetTargetTime(BASE_SHOOTING_DUR);

                shootTimer.ResetTimer(true);
            }
        }

        private void UpdateLocation(GameTime gameTime)
        {
            // check if speed buff
            if (buffs[DOUBLE_SPEED_INDEX]) speed = 2 * BASE_SPEED;
            else speed = BASE_SPEED;

            // update player position base on key presses
            if (Game1.prevKb.IsKeyDown(Keys.Left) || Game1.prevKb.IsKeyDown(Keys.A)) loc.X -= speed;
            else if (Game1.prevKb.IsKeyDown(Keys.Right) || Game1.prevKb.IsKeyDown(Keys.D)) loc.X += speed;

            // clamp player position and set as rectangle
            loc.X = MathHelper.Clamp(loc.X, 0, Game1.SCREEN_WIDTH - WIDTH);
            rec.X = (int)loc.X;
        }

        public void Draw()
        {
            spriteBatch.Draw(skin, rec, Color.White);
        }

        public void ResetBuffs()
        {
            for (int i = 0; i < NUM_UPGRADES; i++) buffs[i] = false;

            damage = BASE_DAMAGE;
            shootTimer.SetTargetTime(BASE_SHOOTING_DUR);
            speed = BASE_SPEED;
        }

        public void AddBuff(int buffIndex)
        {
            buffs[buffIndex] = true;
        }
    }
}
