using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace FrunWithXNA2
{
    public interface ICollider
    {

        void CollisionInteraction(object collider);

        Rectangle GetCollisionArea();

    }
}
