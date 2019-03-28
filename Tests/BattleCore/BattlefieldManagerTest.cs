﻿using System.Collections.Generic;
using HexCore.BattleCore;
using HexCore.DataStructures;
using HexCore.HexGraph;
using NUnit.Framework;

namespace Tests.BattleCore
{
    [TestFixture]
    public class BattlefieldManagerTest
    {
        [Test]
        public void ShouldBeAbleToRunBattleFromTheBeginningToTheEnd()
        {
            //This is an integration test for battlefild, aimed to help to design structure and interfaces.

            // 1. Let's initialize the map:
            var map = GraphFactory.CreateSquareGraph(height: 10, width: 10);

            // 2. Let's create some units. Let's consider one meele, one ranged and one mage on each side.
            // We also need to manage different teams.
            // or battlefiled.GetAllUnitsThatCanPerformActionsByTeamId(); (Looks shitty, definitly need a better name :D)
            // Maybe it also can be useful to have something like:

            // battlefield.GetAllTeamIds();
            // battlefield.GetTeamById();
            // battlefield.IsTeamTurn(teamId);

            // Some distant plans: 
            // team.AddUnit()
            // team.GetAllUnits();
            // team.GetAllUnitsIds();
            // ?? team.GetAllUnitsPositions();  

            var fireBall = new Ability(2, 1);

            var redMelee = new Pawn("red_meele", new Coordinate2D(0, 1, OffsetTypes.OddRowsRight).To3D(),
                BasicMovementTypes.Ground, 6, 1, "red_team");
            var redRange = new Pawn("red_range", new Coordinate2D(1, 0, OffsetTypes.OddRowsRight).To3D(),
                BasicMovementTypes.Ground, 3, 2, "red_team");
            var redMage = new Pawn("red_mage", new Coordinate2D(0, 0, OffsetTypes.OddRowsRight).To3D(),
                BasicMovementTypes.Ground, 3, 1, "red_team");
            var redTeam = new Team(new List<Pawn> {redMelee /*redRange, redMage*/}, "red_team");


            var blueMelee = new Pawn("blue_meele", new Coordinate2D(9, 8, OffsetTypes.OddRowsRight).To3D(),
                BasicMovementTypes.Ground, 6, 1, "blue_team");
            var blueRange = new Pawn("blue_range", new Coordinate2D(8, 9, OffsetTypes.OddRowsRight).To3D(),
                BasicMovementTypes.Ground, 3, 2, "blue_team");
            var blueMage = new Pawn("blue_mage", new Coordinate2D(9, 9, OffsetTypes.OddRowsRight).To3D(),
                BasicMovementTypes.Ground, 3, 1, "blue_team");
            var blueTeam = new Team(new List<Pawn> {blueMelee /*, blueRange, blueMage*/}, "blue_team");

            // Think about more convenient way to do this
            redMage.Abilities.Add(fireBall);
            blueMage.Abilities.Add(fireBall);

            // 3. Let's add our units to the battlefield
            var battlefieldManager = new BattlefieldManager(map, new List<Team> {redTeam, blueTeam});

            // I think it's a good idea to have something that starts the game and checks if everything is set
            battlefieldManager.StartBattle();

            // todo: we need to initialize units positions
            // For now let's consider that red team is 0,0; 0,1; 1,0 and blue team is 9,9; 9,8; 8,9;
            // Let's think how battle should go: meelee should be trying to get as close as possible,
            // range to run from meele and attacking from the distance, same for the mage.
            // Also it should not be possible for team to move it's units when it's not their turn.
            // (Probably it's good to have something like 'FinishTurn' method, that can be called automatically when no
            // more actions could be performed or invoked manually)

            // Before actually performing the move, we need to display possible movement range.


            // TODO: uncomment
            // battlefieldManager.GetPawnMovementRange(redTeam.Id, redMelee.Id);

            // Desirable locations would be: 4,4 for red meele, 5,3 for for range and 4,3 for mage
            // For the blue team - 5,4 for blue meelee, 4, 5 for range and 5,5 for mage
            // Let's assume that fisrt turn is the red's team turn;

//            var redMeleePath = battlefieldManager.Graph.GetShortestPath(redMelee.Position, new Coordinate2D(4,4)), redMelee.MovementType);
//            var redRangePath = battlefieldManager.Graph.GetShortestPath(redRange.Position, new Coordinate2D(5,3)), redRange.MovementType);
//            var redMagePath = battlefieldManager.Graph.GetShortestPath(redMage.Position, new Coordinate2D(4,3)), redMage.MovementType);           
//            
//            var blueMeleePath = battlefieldManager.Graph.GetShortestPath(blueMelee.Position, new Coordinate2D(4,4)), blueMelee.MovementType);
//            var blueRangePath = battlefieldManager.Graph.GetShortestPath(blueRange.Position, new Coordinate2D(5,3)), blueRange.MovementType);
//            var blueMagePath = battlefieldManager.Graph.GetShortestPath(blueMage.Position, new Coordinate2D(4,3)), blueMage.MovementType);

            var initialPosition = new Coordinate2D(0, 1, OffsetTypes.OddRowsRight).To3D();
            var moveTo = new Coordinate2D(4, 4, OffsetTypes.OddRowsRight).To3D();

            Assert.That(redMelee.Position, Is.EqualTo(initialPosition));
            var isMoved =
                battlefieldManager.MovePawn(redMelee, new Coordinate2D(4, 4, OffsetTypes.OddRowsRight).To3D());
            Assert.IsTrue(isMoved);
            Assert.That(redMelee.Position, Is.EqualTo(moveTo));
            Assert.IsTrue(battlefieldManager.Graph.GetCellState(moveTo).IsBlocked);
            Assert.IsFalse(battlefieldManager.Graph.GetCellState(initialPosition).IsBlocked);

            // battlefieldManager.MovePawn(redRange, new Coordinate2D { X = 5, Y = 3}));
            // battlefieldManager.MovePawn(redMage, new Coordinate2D { X = 4, Y = 3}));

            // Now red's team turn should be over; Time to move blue team
            isMoved = battlefieldManager.MovePawn(blueMelee, new Coordinate2D(5, 4, OffsetTypes.OddRowsRight).To3D());
            Assert.IsTrue(isMoved);

            // battlefieldManager.MovePawn(blueRange, new Coordinate2D { X = 4, Y = 5}));
            // battlefieldManager.MovePawn(blueMage, new Coordinate2D { X = 5, Y = 5}));

            // Repeat until we are close enough
            // To show the list of possible options
            var attackablePawns = battlefieldManager.GetAllAttackablePawnsByPawn(redMelee);

            Assert.IsTrue(attackablePawns.Contains(blueMelee));
            Assert.That(blueMelee.HealthPoints, Is.EqualTo(2));

            var attackResult = battlefieldManager.PerformPhysicalAttack(redMelee, blueMelee);

            Assert.That(attackResult.DamageDealt, Is.EqualTo(1));
            Assert.IsTrue(attackResult.IsTargetPawnStillAlive);
            Assert.That(blueMelee.HealthPoints, Is.EqualTo(1));

            // Attack back
            attackResult = battlefieldManager.PerformPhysicalAttack(blueMelee, redMelee);
            attackResult = battlefieldManager.PerformPhysicalAttack(redMelee, blueMelee);

            Assert.That(attackResult.DamageDealt, Is.EqualTo(1));
            Assert.That(blueMelee.HealthPoints, Is.EqualTo(0));
            Assert.IsFalse(attackResult.IsTargetPawnStillAlive);

            // Now we need the mage to cast a damaging spell
            // First, we need to understand range

            // TODO: let's do it without mages first.
//            var possibleAbilityTargets = battlefieldManager.GetPossibleAbilityTargets(blueMage, fireBall);
//            var castResult = battlefieldManager.UseAbility(blueMage, fireBall, redMage);

            // Do all remaining actions until one of the teams wins.
        }
    }
}