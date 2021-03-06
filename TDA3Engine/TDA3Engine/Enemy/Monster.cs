﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using ParticleSystemLibrary;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace TDA3Engine
{
    public enum TypeElement { 
        Flying, Armor, Subterrain, Normal, Invisible 
    }
    public class Monster : GameplayObject
    {
        [ContentTypeWriter]
        public class MonsterWriter : ContentTypeWriter<Monster>
        {
            GameplayObjectWriter gameobjectWriter = null;

            protected override void Initialize(ContentCompiler compiler)
            {
                gameobjectWriter = compiler.GetTypeWriter(typeof(GameplayObject)) as GameplayObjectWriter;

                base.Initialize(compiler);
            }
            protected override void Write(ContentWriter output, Monster value)
            {
                output.WriteRawObject<GameplayObject>(value as GameplayObject, gameobjectWriter);
                output.Write(value.Type.ToString());
                output.Write(value.Health);
                output.Write(value.TextureAsset);
                output.Write(value.HitTextureAsset);
                output.Write(value.HitCueName);
                output.Write(value.IsBoss);
            }

            public override string GetRuntimeReader(Microsoft.Xna.Framework.Content.Pipeline.TargetPlatform targetPlatform)
            {
                return typeof(Monster.MonsterReader).AssemblyQualifiedName;
            }
        }

        [ContentSerializer]
        public TypeElement Type
        {
            get;
            private set;
        }

        [ContentSerializer]
        public int Health
        {
            get;
            private set;
        }

        [ContentSerializerIgnore]
        public int MaxHealth
        {
            get;
            private set;
        }

        [ContentSerializer]
        private string TextureAsset
        {
            get;
            set;
        }

        [ContentSerializer]
        private string HitTextureAsset
        {
            get;
            set;
        }

        [ContentSerializerIgnore]
        public Texture2D HitTexture
        {
            get;
            private set;
        }

        [ContentSerializer]
        public string HitCueName
        {
            get;
            private set;
        }

        [ContentSerializer(Optional = true)]
        public bool IsBoss
        {
            get;
            private set;
        }

        [ContentSerializerIgnore]
        public Wave Wave
        {
            get;
            set;
        }

        [ContentSerializerIgnore]
        public int PathIndex
        {
            get;
            protected set;
        }

        [ContentSerializerIgnore]
        public float DistanceToTravel
        {
            get;
            private set;
        }

        [ContentSerializerIgnore]
        public float DistanceTraveled
        {
            get;
            private set;
        }

        [ContentSerializerIgnore]
        public float Delay
        {
            get;
            set;
        }

        [ContentSerializerIgnore]
        public bool IsActive
        {
            get { return Delay <= 0 && Status == ObjectStatus.Active; }
        }

        float hittimer;
        float statisticstimer;

        Text hitDisplay;

        float deltahealth;
        float dottimer;
        float dottime;
        float dostime;
        float dostimer;
        float upgradetime;
        float upgradetimer;
        float speedOriginal;
        bool DOSA = false;
        int dothealth;
        TypeElement tipoAnterior;

        public event EventHandler DieEvent;
        public event EventHandler HitEvent;

        public Monster()
        {
            DistanceToTravel = float.MinValue;
            DistanceTraveled = float.MaxValue;
        }

        public override void Update(GameTime gameTime)
        {
            if (dothealth > 0 && dottime > 0)
            {
                UpdateDOT(gameTime);
                if (dottimer > dottime)
                {
                    deltahealth = 0.0f;
                    dothealth = 0;
                    dottime = 0.0f;
                    dottimer = 0.0f;
                }
            }

            if (dostime > 0)
            {
                UpdateDOS(gameTime);
                if (dostimer > dostime)
                {
                    Speed = speedOriginal;
                    DOSA = false;
                    dostime = 0.0f;
                    dostimer = 0.0f;
                }
            }

            if (upgradetime > 0)
            {
                UpdateUpgrade(gameTime);
                if (upgradetimer > upgradetime)
                {
                    Type = tipoAnterior;
                    upgradetime = 0.0f;
                    upgradetimer = 0.0f;
                }
            }

            if (hittimer > 0)
            {
                hittimer -= (float)gameTime.ElapsedGameTime.TotalSeconds * Session.singleton.Speed;
                hitDisplay.Update(gameTime);

                if (hittimer <= 0)
                {
                    hitDisplay = null;
                }
            }

            if (Delay > 0)
            {
                Delay -= (float)gameTime.ElapsedGameTime.TotalSeconds * Session.singleton.Speed;
            }
            else
            {
                PathFinding ShortestPath = Wave.Path.ShortestPath;

                if (PathIndex == ShortestPath.path.Count && DistanceTraveled > DistanceToTravel)
                {
                    Die();
                    Wave.Remove(this);
                }
                else
                {
                    int i = PathIndex + 1;
                    if (DistanceTraveled > DistanceToTravel)
                    {
                        if (i < ShortestPath.path.Count)
                        {
                            NewNodeInPath(ShortestPath.path[i]);
                        }
                        else
                        {
                            NewNodeInPath(Wave.Path.End);
                        }
                    }
                    else
                    {
                        DistanceTraveled += (Speed * (float)gameTime.ElapsedGameTime.TotalSeconds * Session.singleton.Speed);
                    }
                }

                base.Update(gameTime);
            }
        }

        private void UpdateDOT(GameTime gameTime)
        {
            float deltaseconds = (float)(gameTime.ElapsedGameTime.TotalSeconds * Session.singleton.Speed);
            dottimer += deltaseconds;
            deltahealth += dothealth * deltaseconds;
            int damage = (int)deltahealth;
            if (damage > 0)
            {
                Health -= damage;
                deltahealth = deltahealth - damage;

                if (Health <= 0)
                {
                    Wave.Remove(this);
                    Die();
                    Session.singleton.AddMoney(IsBoss ? (int)(Wave.MoneyPerKill * Wave.BossMoneyScalar) : Wave.MoneyPerKill);
                    if (DieEvent != null)
                        DieEvent(this, EventArgs.Empty);
                }
                else
                {
                    hittimer = 0.2f;
                    hitDisplay = new Text(damage.ToString(), new Vector2(Rectangle.Right + 3, Rectangle.Top), new Vector2(Velocity.X, -Speed));
                }
            }
        }

        private void UpdateDOS(GameTime gameTime)
        {
            float deltaseconds = (float)(gameTime.ElapsedGameTime.TotalSeconds * Session.singleton.Speed);
            dostimer += deltaseconds;
            deltahealth += dothealth * deltaseconds;
            int damage = (int)deltahealth;
            if (damage > 0)
            {
                Health -= damage;
                deltahealth = deltahealth - damage;

                if (Health <= 0)
                {
                    Wave.Remove(this);
                    Die();
                    Session.singleton.AddMoney(IsBoss ? (int)(Wave.MoneyPerKill * Wave.BossMoneyScalar) : Wave.MoneyPerKill);
                    if (DieEvent != null)
                        DieEvent(this, EventArgs.Empty);
                }
                else
                {
                    hittimer = 0.2f;
                    hitDisplay = new Text(damage.ToString(), new Vector2(Rectangle.Right + 3, Rectangle.Top), new Vector2(Velocity.X, -Speed));
                }
            }
        }

        internal void ApplyDOT(int damage, float time)
        {
            if (dothealth == 0 && dottime == 0)
            {
                deltahealth = 0.0f;
                dothealth = damage;
                dottime = time;
                dottimer = 0.0f;
            }
        }

        internal void ApplyDOS(float speedy, float time)
        {
            if (dostime == 0 && !DOSA)
            {
                DOSA = true;
                speedOriginal = Speed;
                Speed = (Speed*(1-speedy));
                dostime = time;
                dostimer = 0.0f;
            }
        }

        public void NewNodeInPath(Tile t)
        {
            PathIndex++;
            Point tilePosition = Wave.Map.ToWorldCoordinates(t);
            Vector2 newVelocity = new Vector2(tilePosition.X - Position.X, tilePosition.Y - Position.Y);
            DistanceToTravel = newVelocity.Length();
            DistanceTraveled = 0;
            newVelocity.Normalize();
            Velocity = Vector2.Multiply(newVelocity, Speed);
            //Rotation = (float)Math.Atan2(Velocity.Y, Velocity.X);
            Rotation = (float)0;
        }

        public void AddToWave(Wave w)
        {
            Wave = w;
        }

        internal void ApplyUpgrade(TypeElement ta, TypeElement tn)
        {
            if (upgradetime == 0)
            {
                upgradetime = 2;
                upgradetimer = 0.0f;
                tipoAnterior = ta;
                Type = tn;
            }
        }

        private void UpdateUpgrade(GameTime gameTime)
        {
            float deltaseconds = (float)(gameTime.ElapsedGameTime.TotalSeconds * Session.singleton.Speed);
            upgradetimer += deltaseconds;
        }

        public void hit(Bullet bullet, Tower Owner)
        {
            switch(bullet.BType){
                case TypeElement.Invisible:
                    switch (Type){
                        case TypeElement.Invisible:
                            if (Owner.upgraded)
                            {
                                Type = TypeElement.Normal;
                                damage(Owner, 1);
                            }
                            else
                            {
                                damage(Owner, 1);
                            }
                            break;
                        case TypeElement.Flying:
                            damage(Owner, 0);
                            break;
                        case TypeElement.Normal:
                            damage(Owner, 1.2f);
                            break;
                    }
                    break;
                case TypeElement.Flying:
                    switch (Type)
                    {
                        case TypeElement.Armor:
                            damage(Owner, 0.2f);
                            break;
                        case TypeElement.Flying:
                            if (Owner.upgraded)
                            {
                                ApplyUpgrade(Type, TypeElement.Normal);
                                damage(Owner, 1);
                            }
                            else
                            {
                                damage(Owner, 1);
                            }
                            break;
                        case TypeElement.Normal:
                            damage(Owner, 0.8f);
                            break;
                    }
                    break;
                case TypeElement.Normal:
                    switch (Type)
                    {
                        case TypeElement.Armor:
                            damage(Owner, 0.2f);
                            break;
                        case TypeElement.Flying:
                            damage(Owner, 0);
                            break;
                        case TypeElement.Normal:
                            if (Owner.upgraded)
                            {
                                damage(Owner, 5);
                            }
                            else
                            {
                                damage(Owner, 1);
                            }
                            break;
                    }
                    break;
                case TypeElement.Subterrain:
                    switch (Type)
                    {
                        case TypeElement.Armor:
                            damage(Owner, 0);
                            break;
                        case TypeElement.Flying:
                            damage(Owner, 0);
                            break;
                        case TypeElement.Subterrain:
                            damage(Owner, 5);
                            break;
                        case TypeElement.Normal:
                            if (Owner.upgraded)
                            {
                                damage(Owner, 5);
                            }
                            else
                            {
                                damage(Owner, 1);
                            }
                            break;
                    }
                    break;
            }
            //AudioManager.singleton.PlaySound(HitCueName);
            if (bullet.BType == TypeElement.Normal && Type == TypeElement.Normal)
            {
                damage(Owner, 1);
            }
        }

        public void damage(Tower Owner, float multiplier)
        {
            Health -= Convert.ToInt32(Owner.CurrentStatistics.Damage * multiplier);
            if (Health <= 0)
            {
                Wave.Remove(this);
                Die();
                Session.singleton.AddMoney(IsBoss ? (int)(Wave.MoneyPerKill * Wave.BossMoneyScalar) : Wave.MoneyPerKill);
                if (DieEvent != null)
                    DieEvent(this, EventArgs.Empty);
            }
            else
            {
                hittimer = 0.4f;
                hitDisplay = new Text((Owner.CurrentStatistics.Damage*multiplier).ToString(), new Vector2(Rectangle.Right + 3, Rectangle.Top), new Vector2(Velocity.X, -Speed));
            }
        }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (Delay <= 0)
            {
                Texture2D t = Texture;

                if (hittimer > 0) Texture = HitTexture;
                base.Draw(gameTime, spriteBatch);
                if (hittimer > 0 && hitDisplay != null) hitDisplay.Draw(spriteBatch, Session.singleton.UI.Font, Session.singleton.Map.ForeColor);
                Texture = t;
            }
        }

        public class MonsterReader : ContentTypeReader<Monster>
        {
            protected override Monster Read(ContentReader input, Monster existingInstance)
            {
                Monster result = new Monster();

                input.ReadRawObject<GameplayObject>(result as GameplayObject);
                result.Type = (TypeElement)Enum.Parse(typeof(TypeElement), input.ReadString());
                result.Health = input.ReadInt32();
                result.MaxHealth = result.Health;
                result.TextureAsset = input.ReadString();
                result.Texture = input.ContentManager.Load<Texture2D>(String.Format("Textures\\Monsters\\{0}", result.TextureAsset));
                result.HitTextureAsset = input.ReadString();
                result.HitTexture = input.ContentManager.Load<Texture2D>(String.Format("Textures\\Monsters\\{0}", result.HitTextureAsset));
                result.HitCueName = input.ReadString();
                result.IsBoss = input.ReadBoolean();
                result.PathIndex = 0;

                return result;
            }
        }


        public Monster Clone()
        {
            Monster result = new Monster();

            result.Name = Name;
            result.Type = Type;
            result.Description = Description;
            result.Alpha = Alpha;
            result.Speed = Speed;
            result.Health = Health;
            result.MaxHealth = MaxHealth;
            result.TextureAsset = TextureAsset;
            result.Texture = Texture;
            result.HitTextureAsset = HitTextureAsset;
            result.HitCueName = HitCueName;
            result.HitTexture = HitTexture;
            result.IsBoss = IsBoss;
            result.PathIndex = -1;

            return result;
        }
    }
}
