#region Disclaimer / License
//  Copyright (C) 2012, Aaron Hamid
//  https://github.com/ahamid, aaron.hamid@gmail.com
//  
//  This library is free software; you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as
//  published by the Free Software Foundation; either version 2.1 of the
//  License, or (at your option) any later version.
// 
//  This library is distributed in the hope that it will be useful, but
//  WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
//  Lesser General Public License for more details.
// 
//  You should have received a copy of the GNU Lesser General Public
//  License along with this library; if not, write to the Free Software
//  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
#endregion
using System;
using System.Data.LightDatamodel;
using Duplicati.Library.Utility;
using Duplicati.Datamodel;
using Duplicati.Server;

namespace Duplicati_Test
{
    // Base class for Duplicati NUnit tests
    public abstract class BaseDuplicatiTest
    {
        // helper that invokes a closure in the context of a temporary folder
        protected static void withTempFolder(Action<TempFolder> action)
        {
            using(TempFolder tf = new TempFolder())
            {
                action(tf);
            }
        }
        
        // helper that invokes a closure with a loaded test Duplicati applications settings database
        protected static void withApplicationSettingsDb(TempFolder tf, Action<TempFolder, ApplicationSettings> action)
        {
            using(System.Data.IDbConnection con = (System.Data.IDbConnection)Activator.CreateInstance(Duplicati.Server.SQLiteLoader.SQLiteConnectionType))
            {
                Duplicati.GUI.Program.OpenSettingsDatabase(con, tf, "Duplicati_Test.sqlite");
                
                var dataFetcher = new DataFetcherWithRelations(new SQLiteDataProvider(con));
                var appSettings = new ApplicationSettings(dataFetcher);
                
                action(tf, appSettings);
                
                dataFetcher.CommitRecursive(dataFetcher.GetObjects<ApplicationSetting>());
            }
        }
    
        // helper that invokes a closure with a new loaded test Duplicati application settings database
        protected static void withNewApplicationSettingsDb(Action<TempFolder, ApplicationSettings> action)
        {
            withTempFolder((tf) => {
                withApplicationSettingsDb(tf, action);
            });
        }
    }
}