Voici un exemple complet de README en français pour votre projet GitHub "TvPourTous" :

---

# TvPourTous

**TvPourTous** est une application IPTV développée en WPF avec C#. Elle permet de lire des flux vidéo en direct à partir de fichiers M3U, de gérer dynamiquement ces sources et d'afficher le contenu en mode plein écran exclusif. L'application utilise [LibVLCSharp](https://github.com/videolan/libvlcsharp) pour la lecture vidéo et stocke la liste des sources M3U dans un fichier JSON afin d'assurer leur persistance.

## Table des matières

- [Fonctionnalités](#fonctionnalités)
- [Technologies utilisées](#technologies-utilisées)
- [Prérequis](#prérequis)
- [Installation](#installation)
- [Utilisation](#utilisation)
- [Personnalisation](#personnalisation)
- [Contribuer](#contribuer)
- [Licence](#licence)
- [Auteurs](#auteurs)

## Fonctionnalités

- **Lecture IPTV** : Lire des flux vidéo en direct à partir d'une liste de fichiers M3U.
- **Gestion dynamique des sources** : Ajouter, modifier et supprimer des sources M3U via une interface conviviale.
- **Stockage persistant** : Les sources M3U sont stockées dans un fichier JSON (`m3u.json`) pour conserver vos réglages entre les sessions.
- **Mode plein écran exclusif** : Passage en plein écran avec arrêt du flux dans la fenêtre principale et relance automatique du flux à la sortie du mode plein écran.
- **Interface moderne et sobre** : Design épuré avec un thème sombre et des contrôles stylisés.
- **Recherche et filtrage** : Possibilité de rechercher des chaînes dans la liste affichée.

## Technologies utilisées

- **WPF** pour l'interface utilisateur
- **C#** et **.NET** (Framework ou Core selon votre configuration)
- **LibVLCSharp** pour la lecture vidéo
- **JSON** pour la gestion des sources M3U

## Prérequis

- **Système d'exploitation** : Windows 7 ou ultérieur
- **.NET Framework** (version 4.7.2 ou supérieure) ou **.NET Core / .NET 5/6**
- **LibVLC** (la bibliothèque native VLC doit être installée ou accessible via NuGet)

## Installation

1. **Télécharger la Release**

2. **Exectuer l'exe**

## Utilisation

![image_2025-03-02_023530226](https://github.com/user-attachments/assets/fd7b2860-5f82-4cf3-bb9b-5c9a60f3464f)

- **Sélection de la source M3U** :  
  Dans la ComboBox, choisissez une source M3U parmi celles affichées.  
- **Recherche** :  
  Utilisez le champ de recherche pour filtrer la liste des chaînes.
- **Lecture** :  
  Cliquez sur une chaîne pour lancer la lecture dans le lecteur vidéo intégré.
- **Gestion des sources** :  
  Cliquez sur le bouton "Gérer les sources" pour ouvrir l'interface de gestion où vous pouvez ajouter, modifier ou supprimer des sources M3U.
- **Mode plein écran** :  
  Appuyez sur "Plein Écran" pour lancer le flux en mode plein écran. Quittez le mode plein écran en appuyant sur `Échap` : le flux se coupera et redémarrera dans la fenêtre principale.

## Personnalisation

- **Ajout de sources M3U** :  
  Vous pouvez ajouter vos propres sources via l'interface "Gérer les sources" ou en éditant directement le fichier `m3u.json`.
- **Thème et Styles** :  
  Les styles de l'interface (couleurs, polices, etc.) sont définis dans les fichiers XAML. Vous pouvez les personnaliser pour adapter l'apparence à vos préférences.

## Contribuer

Les contributions sont les bienvenues ! Pour contribuer :

1. **Fork** le dépôt.
2. **Créez une branche** pour vos modifications (`git checkout -b feature/mon-nouvelle-fonctionnalite`).
3. **Validez vos modifications** (`git commit -am 'Ajout d'une nouvelle fonctionnalité'`).
4. **Poussez la branche** (`git push origin feature/mon-nouvelle-fonctionnalite`).
5. **Créez une Pull Request** sur GitHub.

## Licence

Ce projet est sous licence [MIT](LICENSE).

## Auteurs

- **Antunes Rodrigue** – *Développeur principal* – [rodrigueantunes](https://github.com/rodrigueantunes)

N'hésitez pas à me contacter pour toute question ou suggestion !

---

Ce README offre une vue d'ensemble complète du projet, des fonctionnalités aux instructions d'installation et de contribution. Vous pouvez bien entendu l'ajuster selon les spécificités et évolutions de votre application.
