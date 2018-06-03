﻿using System;
using System.Collections.Generic;
using HexCore.DataStructures;
using HexCore.HexGraph;

namespace HexCore.BattleCore.Unit
{
    [Serializable]
    public class UnitController : AbstractUnitController
    {
        private readonly Graph _graph;

        public UnitController(UnitState unitState, Graph graph)
        {
            State = unitState;
            _graph = graph;
        }

        public override UnitState State { get; }

        public override List<Coordinate3D> GetAttackRange()
        {
            return _graph.GetRange(State.Position, State.Attack.Range);
        }

        public override List<Coordinate3D> GetMovementRange()
        {
            return _graph.GetMovementRange(State.Position, State.MovementPoints, State.MovementType);
        }

        public override bool CanMoveTo(Coordinate3D position)
        {
            return GetMovementRange().Contains(position);
        }

        public override bool CanAttack(AbstractUnitController unit)
        {
            return GetAttackRange().Contains(unit.State.Position);
        }

        public override void Attack(AbstractUnitController unit)
        {
            throw new NotImplementedException();
        }
    }
}