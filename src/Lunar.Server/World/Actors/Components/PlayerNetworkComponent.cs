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

using System;
using Lidgren.Network;
using Lunar.Core;
using Lunar.Core.Net;
using Lunar.Core.World;
using Lunar.Server.Net;
using Lunar.Server.Utilities.Commands;

namespace Lunar.Server.World.Actors.Components
{
    public class PlayerNetworkComponent
    {
        private readonly Player _player;

        public PlayerConnection Connection { get; }

        public PlayerNetworkComponent(Player player, PlayerConnection connection)
        {
            _player = player;
            this.Connection = connection;
        }

        public void SendAvailableCommands()
        {
            var packet = new Packet(PacketType.AVAILABLE_COMMANDS, ChannelType.UNASSIGNED);
            packet.Message.Write(Engine.Services.Get<CommandHandler>().Pack());
            this.SendPacket(packet, NetDeliveryMethod.ReliableOrdered);
        }

        public void SendPositionUpdate()
        {
            var packet = new Packet(PacketType.POSITION_UPDATE, ChannelType.UNASSIGNED);
            packet.Message.Write(_player.UniqueID);
            packet.Message.Write(_player.Layer.Name);
            packet.Message.Write(_player.Descriptor.Position);
            _player.Map.SendPacket(packet, NetDeliveryMethod.ReliableOrdered);
        }

        public void SendPlayerData()
        {
            var packet = new Packet(PacketType.PLAYER_DATA, ChannelType.UNASSIGNED);
            packet.Message.Write(_player.Pack());
            this.SendPacket(packet, NetDeliveryMethod.ReliableOrdered);
        }

        public void SendChatMessage(string message, ChatMessageType type)
        {
            var packet = new Packet(PacketType.PLAYER_MSG, ChannelType.UNASSIGNED);

            packet.Message.Write((byte)type);
            packet.Message.Write(message);

            this.SendPacket(packet, NetDeliveryMethod.Unreliable);
        }

        public void SendPacket(Packet packet, NetDeliveryMethod method)
        {
            _player.NetworkComponent.Connection.SendPacket(packet, method);
        }

        public void SendPlayerStats()
        {
            var packet = new Packet(PacketType.PLAYER_STATS, ChannelType.UNASSIGNED);
            packet.Message.Write(_player.UniqueID);
            packet.Message.Write(_player.Descriptor.Speed);
            packet.Message.Write(_player.Descriptor.Level);
            packet.Message.Write(_player.Descriptor.Stats.CurrentHealth);
            packet.Message.Write(_player.Descriptor.Stats.Health);
            packet.Message.Write(_player.Descriptor.Stats.Strength + _player.Descriptor.StatBoosts.Strength);
            packet.Message.Write(_player.Descriptor.Stats.Intelligence + _player.Descriptor.StatBoosts.Intelligence);
            packet.Message.Write(_player.Descriptor.Stats.Dexterity + _player.Descriptor.StatBoosts.Dexterity);
            packet.Message.Write(_player.Descriptor.Stats.Defense + _player.Descriptor.StatBoosts.Defense);
            _player.Map.SendPacket(packet, NetDeliveryMethod.ReliableOrdered);
        }

        public void SendInventoryUpdate()
        {
            var packet = new Packet(PacketType.INVENTORY_UPDATE, ChannelType.UNASSIGNED);

            for (int i = 0; i < Settings.MaxInventoryItems; i++)
            {
                if (_player.Inventory.GetSlot(i) != null)
                {
                    packet.Message.Write(true); // there is an item in this slot

                    packet.Message.Write(_player.Inventory.GetSlot(i).Item.PackData());
                    packet.Message.Write(_player.Inventory.GetSlot(i).Amount);
                }
                else
                {
                    packet.Message.Write(false);
                }
            }

            this.SendPacket(packet, NetDeliveryMethod.ReliableOrdered);
        }

        public void SendMovementPacket()
        {
            var packet = new Packet(PacketType.PLAYER_MOVING, ChannelType.UNASSIGNED);
            packet.Message.Write(_player.UniqueID);
            packet.Message.Write((byte)_player.Direction);
            packet.Message.Write((byte)_player.State); // true if moving, false if not
            packet.Message.Write(_player.Descriptor.Position);

            _player.Map.SendPacket(packet, NetDeliveryMethod.ReliableOrdered);
        }

        public void SendLoadingScreen(bool active = true)
        {
            var packet = new Packet(PacketType.LOADING_SCREEN, ChannelType.UNASSIGNED);
            packet.Message.Write(active);
            this.SendPacket(packet, NetDeliveryMethod.ReliableOrdered);
        }

        public void SendEquipmentUpdate()
        {
            var packet = new Packet(PacketType.EQUIPMENT_UPDATE, ChannelType.UNASSIGNED);

            for (int i = 0; i < Enum.GetNames(typeof(EquipmentSlots)).Length; i++)
            {
                if (_player.Equipment.GetSlot(i) == null)
                {
                    // There's nothing in this slot.
                    packet.Message.Write(false);
                    continue;
                }

                packet.Message.Write(true);
                packet.Message.Write(_player.Equipment.GetSlot(i).PackData());
            }

            this.SendPacket(packet, NetDeliveryMethod.ReliableOrdered);
        }
    }
}