﻿/** Copyright 2018 John Lamontagne https://www.rpgorigin.com

	Licensed under the Apache License, Version 2.0 (the "License");
	you may not use this file except in compliance with the License.
	You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0

	Unless required by applicable law or agreed to in writing, software
	distributed under the License is distributed on an "AS IS" BASIS,
	WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	See the License for the specific language governing permissions and
	limitations under the License.
*/

using Lunar.Core.Content.Graphics;
using Lunar.Core.Utilities.Data;
using Lunar.Core.Utilities.Logic;
using System;
using System.IO;

namespace Lunar.Core.World.Structure
{
    public class LayerDescriptor<T> : ILayerDescriptor<T> where T : ITileDescriptor<SpriteInfo>
    {
        private string _name;
        private int _layerIndex;

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
            }
        }

        public int LayerIndex
        {
            get => _layerIndex;
            set
            {
                _layerIndex = value;

                foreach (var tile in this.Tiles)
                    if (tile != null && tile.Sprite != null)
                        tile.Sprite.Transform.LayerDepth = this.LayerIndex;
            }
        }

        public float ZIndex { get => this.LayerIndex * EngineConstants.PARTS_PER_LAYER; }

        public T[,] Tiles { get; protected set; }

        public LayerDescriptor(Vector dimensions, string layerName, int lIndex)
        {
            this.Tiles = new T[(int)dimensions.X, (int)dimensions.Y];

            this.Name = layerName;
            this.LayerIndex = lIndex;
        }

        protected LayerDescriptor()
        {
        }

        public void Resize(Vector dimensions)
        {
            this.Tiles = Helpers.ResizeArray<T>(this.Tiles, (int)dimensions.X, (int)dimensions.Y);
        }
    }
}