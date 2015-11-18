 Authors:
 * Eric Longberg
 * Tivinia Pohahau
 
 Last Update:
 * 17 November 2015
 
 
 INTRODUCTION
 ------------
 
 AgCubio is a fun game where you must eat or be eaten! There are only two rules:
 1) Players can only consume cubes that are smaller than their cube
 2) Stay away from larger cubes or you will be eaten
 Along with those rules there are some other things players need to know:
 - When a player consumes a smaller cube, the size of their own cube grows
 - A smaller cube can either be another player or food, which is located throughout the world
 - A player can split his/her cube by pressing the 'Space' bar
 Try to be the last player standing by becoming the biggest cube in the world.
 
 REQUIREMENTS
 ------------
 
 This game requires the following things before a player enters the AgCubio world:
  * Server for player to connect to (default is localhost)
  * Player Name: player must enter a name in the Player Name box and press 'Enter'
  
 SETUP DECISIONS
 ---------------
 
 -- View --
 * Decided to use SplitContainer for world and statistics and separate Panel for
   Player Name and Server
 
 * Draw cubes using Graphics, Rectangle, and SolidBrush

 -- Networking --

  
 CHANGELOG
 ---------
 
 -- 1.0 --
 * Initial project setup
 * Installed Json.NET using NuGet
 
 -- 2.0 --
 * Set up the GUI in View
 * Decided to use a Split Container with 2 panels Panel1 (left) and Panel2 (right).
   Panel1 displays the world of AgCubio where cubes are drawn and moved around.
   Panel2 displays the statistics of the game: 
       * Frames per second
	   * Food
	   * Mass
	   * Width
 
 -- 3.0 --
 * Used a separate panel that lies on top of splitContainer1_Panel1.
   It covers the world because it contains two textboxes that ask for:
	   * Player Name
	   * Server (default: localhost)
 * Added code in View to set up connection to Server: Network.Send, Network.i_want_more_data
 * Added Json attributes in Cube and World classes

 -- 4.0 --
 * Added a boolean to keep track of when the player is connected to a server and ready 
   to play
 * Sockets and connections work
 * Updated GUI for displaying cubes