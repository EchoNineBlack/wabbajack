﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Wabbajack.Common.StoreHandlers
{
    public enum StoreType
    {
        STEAM,
        GOG
    }

    public class StoreHandler
    {
        private static readonly Lazy<StoreHandler> instance = new Lazy<StoreHandler>(() => new StoreHandler(), true);
        public static StoreHandler Instance => instance.Value;

        private static readonly Lazy<SteamHandler> _steamHandler = new Lazy<SteamHandler>(() => new SteamHandler());
        public SteamHandler SteamHandler = _steamHandler.Value;

        private static readonly Lazy<GOGHandler> _gogHandler = new Lazy<GOGHandler>(() => new GOGHandler());
        public GOGHandler GOGHandler = _gogHandler.Value;

        public List<AStoreGame> StoreGames;

        public StoreHandler()
        {
            StoreGames = new List<AStoreGame>();

            if (SteamHandler.Init())
            {
                if(SteamHandler.LoadAllGames())
                    StoreGames.AddRange(SteamHandler.Games);
                else
                    Utils.Error(new StoreException("Could not load all Games from the SteamHandler, check previous error messages!"));
            }
            else
            {
                Utils.Error(new StoreException("Could not Init the SteamHandler, check previous error messages!"));
            }

            if (GOGHandler.Init())
            {
                if(GOGHandler.LoadAllGames())
                    StoreGames.AddRange(GOGHandler.Games);
                else
                    Utils.Error(new StoreException("Could not load all Games from the GOGHandler, check previous error messages!"));
            }
            else
            {
                Utils.Error(new StoreException("Could not Init the GOGHandler, check previous error messages!"));
            }
        }

        public string GetGamePath(Game game)
        {
            return StoreGames.FirstOrDefault(g => g.Game == game)?.Path;
        }

        public string GetGamePath(Game game, StoreType type)
        {
            return StoreGames.FirstOrDefault(g => g.Type == type && g.Game == game)?.Path;
        }

        public string GetGamePath(int id)
        {
            return StoreGames.FirstOrDefault(g => g.ID == id)?.Path;
        }
    }

    public abstract class AStoreGame
    {
        public abstract Game Game { get; internal set; }
        public abstract string Name { get; internal set; }
        public abstract string Path { get; internal set; }
        public abstract int ID { get; internal set; }
        public abstract StoreType Type { get; internal set; }
    }

    public abstract class AStoreHandler
    {
        public abstract List<AStoreGame> Games { get; set; }

        public abstract StoreType Type { get; internal set; }

        public abstract bool Init();

        public abstract bool LoadAllGames();
    }

    public class StoreException : Exception
    {
        public StoreException(string msg) : base(msg)
        {

        }
    }
}
