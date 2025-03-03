using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TvPourTous
{
    /// <summary>
    /// Représente l'état d'une chaîne IPTV dans le fichier channels_status.json.
    /// </summary>
    public class ChannelStatus
    {
        /// <summary>
        /// Nom de la playlist M3U d'origine.
        /// </summary>
        [JsonPropertyName("playlist")]
        public string Playlist { get; set; }

        /// <summary>
        /// Nom de la chaîne IPTV.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// État de la chaîne : "v" pour actif (✅) et "x" pour inactif (❌).
        /// </summary>
        [JsonPropertyName("etat")]
        public string Etat { get; set; }
    }
}
