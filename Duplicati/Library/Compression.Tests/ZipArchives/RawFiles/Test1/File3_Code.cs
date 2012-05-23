using System.Collections.Generic;
using DefeatInDetail.Library.GameAgents;
using DefeatInDetail.Library.GameAgents.AgentTypes;
using DefeatInDetail.Library.GameGrid;
using DefeatInDetail.Library.Pathfinding.Common;

namespace DefeatInDetail.Library.Pathfinding.aStar
{
    //TODO: We need checking to see if the target is occupied and if so find the nearest cell
    public class Pathfinder
    {
        #region Properties

        public const bool AllowDiagonalMoves = true;

        /// <summary>
        /// Non diagonal movement cost
        /// </summary>
        private const int OrthogonalCost = 10;

        /// <summary>
        /// Diagonal movement cost
        /// </summary>
        private const int DiagonalCost = 14;

        private IGameGrid _grid { get; set; }
        private AgentList _gameAgents { get; set; }

        /// <summary>
        /// Ordered list of Cells to check
        /// </summary>
        private CalculatedCellList _openList { get; set; }

        /// <summary>
        /// Ordered list of closed/checked cells
        /// </summary>
        private CalculatedCellList _closeList { get; set; }

        #endregion

        #region Init

        public Pathfinder()
        {
            _openList = new CalculatedCellList();
            _closeList = new CalculatedCellList();
        }

        public void Initialize(IGameGrid grid, AgentList agentList)
        {
            _grid = grid;
            _gameAgents = agentList;
        }

        #endregion

        #region Pathfinding

        /// <summary>
        /// The main function in the Pathfinder. This function will work out the quickest route and
        /// update the object with the next valid step for it to move with
        /// Followed guide from http://www.policyalmanac.org/games/aStarTutorial.htm
        /// </summary>
        /// <param name="obj"></param>
        public virtual void ProcessNextStep(MoveableAgent obj)
        {
            CalculatedCell validPath = null;
            if (obj.PathInfo.TargetPosition != null)
            {
                var targetCell = _grid.PositionToCell(obj.PathInfo.TargetPosition.Value);

                Clear();

                //Start the open list with ourself
                _openList.AddOrdered(Shared.GenerateCell(obj, _grid.PositionToCell(obj.Position), null, 0, _grid));

                //Keep looping until we find a path (or we quit the loop)
                while (validPath == null)
                {
                    if (_openList.Count <= 0)
                        break; //No valid path could be found

                    //The list is ordered by F; 0 will always be the smallest F
                    var currentCell = _openList[0];

                    //Lets find a new path/step
                    if (currentCell.CellCoordinates != targetCell)
                    {
                        //Store this cell in the closed list so we dont process it again
                        _closeList.AddOrdered(currentCell);

                        //Remove it from the open list
                        _openList.Remove(_openList[0]);

                        //Find all the empty surrounding cells. As we will be processing all cells we dont need an ordered list
                        List<CalculatedCell> surroundingCells = GetSurroundingEmptyCells(obj, currentCell);

                        //Loop through all found cells 
                        foreach (var t in surroundingCells)
                        {
                            //Do we already have this cell in our open list?
                            var openListCell = _openList.FindCell(t.CellCoordinates);

                            //If we do, check to see if that cell has a better G value
                            //We want the lowest G value
                            if (openListCell != null)
                            {
                                if (t.G < openListCell.G)
                                    openListCell.Parent = currentCell;
                            }
                            else //No one else exists in the open list, lets ADD ME
                                _openList.AddOrdered(t);
                        }

                        //Empty the list
                        surroundingCells.Clear();
                    }
                    else //WE FOUND A VALID PATH                       
                        validPath = currentCell;
                }
            }

            SetNextValidPath(obj, validPath);

            Clear();
        }

        /// <summary>
        /// Assuming there is a valid path the object in question will have the next step assigned to it
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="validPath"></param>
        private void SetNextValidPath(MoveableAgent obj, CalculatedCell validPath)
        {
            //Find next valid step
            if (validPath != null)
            {
                if (validPath.Parent != null)
                {
                    CalculatedCell nextStep = validPath;

                    //While the parent isnt null and the parents paren isnt null, keep searching
                    //This is done to ensure we dont pick up the top parent which is the current
                    //of the object
                    while (nextStep.Parent != null && nextStep.Parent.Parent != null)
                        nextStep = nextStep.Parent;

                    obj.PathInfo.SetNextStep(_grid.CellToPosition(nextStep.CellCoordinates));
                }
                else //TODO: No path was found, do we want to clear the target?
                    obj.PathInfo.ClearPath(); //Cant find a path, stop the object here
            }
            else
                obj.PathInfo.ClearPath(); //Cant find a path, stop the object here
        }

        /// <summary>
        /// Used to find all the empty cells around the current position
        /// </summary>
        /// <returns></returns>
        private List<CalculatedCell> GetSurroundingEmptyCells(MoveableAgent obj, CalculatedCell parentCell)
        {
            //TODO: We are currently assuming objects are only 1 cell big
            var rtnVal = new List<CalculatedCell>();
            Cell currentCellPosition = parentCell.CellCoordinates;

            //Get the cell middle left of the object
            if (currentCellPosition.X > 0)
            {
                var pos = new Cell {X = (currentCellPosition.X - 1), Y = (currentCellPosition.Y)};
                rtnVal.AddRange(TryInsertNewCell(obj, parentCell, pos, OrthogonalCost));
            }

            //Get the cell above the object
            if (currentCellPosition.Y > 0)
            {
                var pos = new Cell {X = currentCellPosition.X, Y = (currentCellPosition.Y - 1)};
                rtnVal.AddRange(TryInsertNewCell(obj, parentCell, pos, OrthogonalCost));
            }

            //Get the cell to the right of the object
            if (currentCellPosition.X < _grid.Columns)
            {
                var pos = new Cell {X = (currentCellPosition.X + 1), Y = (currentCellPosition.Y)};
                rtnVal.AddRange(TryInsertNewCell(obj, parentCell, pos, OrthogonalCost));
            }

            //Get the cells below the object
            if (currentCellPosition.Y < _grid.Rows)
            {
                var pos = new Cell {X = currentCellPosition.X, Y = (currentCellPosition.Y + 1)};
                rtnVal.AddRange(TryInsertNewCell(obj, parentCell, pos, OrthogonalCost));
            }

            //--------
            // Diags
            //--------
            // ReSharper disable ConditionIsAlwaysTrueOrFalse
            if (AllowDiagonalMoves)
                // ReSharper restore ConditionIsAlwaysTrueOrFalse
            {
                //Get the top left cell
                if (currentCellPosition.X > 0 && currentCellPosition.Y > 0)
                {
                    var pos = new Cell {X = (currentCellPosition.X - 1), Y = (currentCellPosition.Y - 1)};
                    rtnVal.AddRange(TryInsertNewCell(obj, parentCell, pos, DiagonalCost));
                }

                //Get the top right
                if (currentCellPosition.X < _grid.Columns && currentCellPosition.Y > 0)
                {
                    var pos = new Cell {X = (currentCellPosition.X + 1), Y = (currentCellPosition.Y - 1)};
                    rtnVal.AddRange(TryInsertNewCell(obj, parentCell, pos, DiagonalCost));
                }

                //Get the bottom right
                if (currentCellPosition.X < _grid.Columns && currentCellPosition.Y < _grid.Rows)
                {
                    var pos = new Cell {X = (currentCellPosition.X + 1), Y = (currentCellPosition.Y + 1)};
                    rtnVal.AddRange(TryInsertNewCell(obj, parentCell, pos, DiagonalCost));
                }

                //Get the bottom left
                if (currentCellPosition.X > 0 && currentCellPosition.Y < _grid.Rows)
                {
                    var pos = new Cell {X = (currentCellPosition.X - 1), Y = (currentCellPosition.Y + 1)};
                    rtnVal.AddRange(TryInsertNewCell(obj, parentCell, pos, DiagonalCost));
                }
            }

            return rtnVal;
        }

        /// <summary>
        /// Attempt to insert a cell into a list
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="parentCell"></param>
        /// <param name="pos"></param>
        /// <param name="cost"></param>
        private IEnumerable<CalculatedCell> TryInsertNewCell(MoveableAgent obj, CalculatedCell parentCell, Cell pos,
                                                             int cost)
        {
            var rtnVal = new List<CalculatedCell>();

            //Dont process a cell that has already been processed
            if (!DoesCellAlreadyExist(pos))
                if (_grid.IsCellEmpty(pos, true, _gameAgents))
                    rtnVal.Add(Shared.GenerateCell(obj, pos, parentCell, cost, _grid));

            return rtnVal;
        }

        /// <summary>
        /// Test to see if the cell in question already exists in either the closed list
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        private bool DoesCellAlreadyExist(Cell pos)
        {
            return (_closeList.FindCell(pos) != null);
        }

        /// <summary>
        /// Clears the open and closed lists
        /// </summary>
        private void Clear()
        {
            _openList.Clear();
            _closeList.Clear();
        }

        #endregion
    }
}