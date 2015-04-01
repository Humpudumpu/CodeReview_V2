﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PVCSTools;
using CodeReview_V2.Model;
using System.Text.RegularExpressions;
using System.ComponentModel;

namespace CodeReview_V2.DataAccess
{
	public class TeamTrackAccess
	{
		const string USER = "CAN10TFSBridgeSvc";
		const string PASSWORD = "Canadatfs.2012";

		TeamTrack tt;
		
		//The checkin's from the incident branch are not recorded in the teamtrack incident.
		//The teamtrack incident only has the branch and merge information.
		//TFS access is required to get the changesets in the incident branch. These changesets will then
		//be converted to ITeamtrack.Association
		TFSAccess tf;
		bool LoggedIn { get; set; }
		public string IncidentTitle { get; set; }
		public string IncidentURL { get; set; }
		

		public TeamTrackAccess()
		{
			tt = new TeamTrack();
			tf = new TFSAccess();
			LoggedIn = false;
			IncidentTitle = String.Empty;
			IncidentURL = @"http://can10-teamtrack/tmtrack/tmtrack.dll?";
		}

		private string GetIncidentTitle(uint incidentNo)
		{
			if (!LoggedIn)
				LoggedIn = tt.Login(USER, PASSWORD);

			return tt.GetIncidentTitle(incidentNo.ToString());
		}

		private string GetIncidentURL(uint incidentNo)
		{
			if (!LoggedIn)
				LoggedIn = tt.Login(USER, PASSWORD);

			return tt.GetIncidentURL(incidentNo);
		}

		private List<ITeamTrack.Association> GetFileAssociations(uint incidentNo)
		{
			if (!LoggedIn)
				LoggedIn = tt.Login(USER, PASSWORD);

			return tt.GetAssociations(incidentNo);
		}

		private List<CustomChangeset> GetIncidentChanges(uint incidentNo)
		{
			List<CustomChangeset> changesets = new List<CustomChangeset>();
			changesets.AddRange(GetChangeset(incidentNo));

			//Get the associations "Incident/#####"
			List<CustomChangeset> incidentBranches = changesets.Where(x => x.IncidentBranch == true).ToList<CustomChangeset>();
			//Get each changeset in the Incident branch
			changesets.AddRange(tf.GetIncidentChanges(incidentBranches));
			return changesets;
		}

		private List<CustomChangeset> GetChangeset(uint incidentNo)
		{
			List<ITeamTrack.Association> associations = GetFileAssociations(incidentNo);
			List<CustomChangeset> changesets = new List<CustomChangeset>();
			Regex incidentBranch = new Regex(@"\$?/Incidents/\d+$");

			//Overthinking here.
			foreach (ITeamTrack.Association association in associations)
			{	CustomChangeset change = null;
				foreach(CustomChangeset changes in changesets)
				{
					if (changes.CheckinChangeSet == association.checkInRevision)
					{
						change = changes;
						change.Files.Add(FileItem.CreateFileItem(association.file));
						break;
					}
				}
				
				//If the changeset exists except for the file in the changeset rest all others will already be present
				//When the changeset does not exist, there is need to create a new changeset object and enter all the details.
				//Assumption is : one changeset is always one author, with one comment, with one checkinrevision but multiple files.
				if (change == null)
				{
				change = new CustomChangeset();
				change.CheckinChangeSet = association.checkInRevision;
				change.CheckoutChangeSet = association.checkOutRevision;
				change.Author = association.author;
				change.Comments = association.logMessage;
				change.Files.Add(FileItem.CreateFileItem(association.file));
				
				//From TeamTrack, find the association that mentions "Incident/#####"
				if (incidentBranch.IsMatch(association.file))
					change.IncidentBranch = true;
				change.DevBranch = "";
				changesets.Add(change);
				}
			}
			return changesets;
		}

		public Incident GetIncident(uint incidentNo)
		{
			Incident incident = new Incident();
			incident.IncidentTitle = GetIncidentTitle(incidentNo);
			incident.IncidentURL = GetIncidentURL(incidentNo);
			//call helper and create the changeset list
			incident.ChangeSets = GetIncidentChanges(incidentNo);
			return incident;
		}
	}
}
