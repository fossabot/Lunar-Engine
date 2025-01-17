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
using Lunar.Core.Utilities;
using Lunar.Core.Utilities.Data;
using Lunar.Core.World.Actor.Descriptors;
using Lunar.Server.Utilities;
using Lunar.Server.World.BehaviorDefinition;
using Lunar.Server.World.Structure;

namespace Lunar.Server.World.Actors
{
    public interface IActor : IActorDescriptor
    {
        string UniqueID { get; }

        bool Attackable { get; }

        bool Alive { get; }
       
        Layer Layer { get; set; }

        ActorBehaviorDefinition Behavior { get; }

        CollisionBody CollisionBody { get; }

        IActor Target { get; set; }

        void Update(GameTime gameTime);

        void WarpTo(Vector position);

        void OnAttacked(IActor attacker, int damageDelt);
    }
}