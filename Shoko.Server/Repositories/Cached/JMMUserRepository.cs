﻿using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using Shoko.Commons.Extensions;
using Shoko.Server.Databases;
using Shoko.Server.Models;

namespace Shoko.Server.Repositories.Cached
{
    public class JMMUserRepository : BaseCachedRepository<SVR_JMMUser, int>
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        protected override int SelectKey(SVR_JMMUser entity) => entity.JMMUserID;

        public override void PopulateIndexes()
        {
        }

        public override void RegenerateDb()
        {
        }

        public override void Save(SVR_JMMUser obj)
        {
            Save(obj, true);
        }

        public void Save(SVR_JMMUser obj, bool updateGroupFilters)
        {
            lock (obj)
            {
                bool isNew = false;
                if (obj.JMMUserID == 0)
                {
                    isNew = true;
                    base.Save(obj);
                }
                if (updateGroupFilters)
                {
                    using (var session = DatabaseFactory.SessionFactory.OpenSession())
                    {
                        SVR_JMMUser old = isNew ? null : session.Get<SVR_JMMUser>(obj.JMMUserID);
                        updateGroupFilters = SVR_JMMUser.CompareUser(old, obj);
                    }
                }
                base.Save(obj);
                if (updateGroupFilters)
                {
                    logger.Trace("Updating group filter stats by user from JMMUserRepository.Save: {0}", obj.JMMUserID);
                    obj.UpdateGroupFilters();
                }
            }
        }


        public SVR_JMMUser GetByUsername(string username)
        {
            lock (GlobalLock)
            {
                return Cache.Values.FirstOrDefault(x =>
                    x.Username.Equals(username, StringComparison.InvariantCultureIgnoreCase));
            }
        }


        public List<SVR_JMMUser> GetAniDBUsers()
        {
            lock (GlobalLock)
            {
                return Cache.Values.Where(a => a.IsAniDBUser == 1).ToList();
            }
        }

        public List<SVR_JMMUser> GetTraktUsers()
        {
            lock (GlobalLock)
            {
                return Cache.Values.Where(a => a.IsTraktUser == 1).ToList();
            }
        }

        public SVR_JMMUser AuthenticateUser(string userName, string password)
        {
            if (password == null) password = string.Empty;
            string hashedPassword = Digest.Hash(password);
            lock (GlobalLock)
            {
                return Cache.Values.FirstOrDefault(a =>
                    a.Username.Equals(userName, StringComparison.InvariantCultureIgnoreCase) &&
                    a.Password.Equals(hashedPassword));
            }
        }

        public bool RemoveUser(int userID, bool skipValidation = false)
        {
            var user = GetByID(userID);
            if (!skipValidation)
            {
                var allAdmins = GetAll().Where(a => a.IsAdminUser()).ToList();
                allAdmins.Remove(user);
                if (allAdmins.Count < 1) return false;
            }

            var toSave = RepoFactory.GroupFilter.GetAll().Select(
                a =>
                {
                    bool changed = false;
                    if (a.GroupsIds.ContainsKey(userID))
                    {
                        a.GroupsIds.Remove(userID);
                        changed = true;
                    }

                    if (a.SeriesIds.ContainsKey(userID))
                    {
                        a.SeriesIds.Remove(userID);
                        changed = true;
                    }

                    if (!changed) return null;
                    a.UpdateEntityReferenceStrings();
                    return a;

                }).Where(a => a != null).ToList();
            RepoFactory.GroupFilter.Save(toSave);

            RepoFactory.AnimeSeries_User.Delete(RepoFactory.AnimeSeries_User.GetByUserID(userID));
            RepoFactory.AnimeGroup_User.Delete(RepoFactory.AnimeGroup_User.GetByUserID(userID));
            RepoFactory.AnimeEpisode_User.Delete(RepoFactory.AnimeEpisode_User.GetByUserID(userID));
            RepoFactory.VideoLocalUser.Delete(RepoFactory.VideoLocalUser.GetByUserID(userID));

            Delete(user);
            return true;
        }

        public long GetTotalRecordCount()
        {
            lock (GlobalLock)
            {
                return Cache.Keys.Count;
            }
        }
    }
}
