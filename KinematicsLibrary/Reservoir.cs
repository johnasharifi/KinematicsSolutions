using UnityEngine;

namespace KinematicsLibrary
{
    public abstract class AbstractReservoir<T>
    {
        protected T reservoir = default;
        protected float leakRate;
        protected float stability;
        public abstract T Exchange(T delta, float dT);
    }

    public class VectorReservoir : AbstractReservoir<Vector3>
    {
        public VectorReservoir(float _stability, float _leakRate)
        {
            stability = _stability;
            leakRate = _leakRate;
        }

        public override Vector3 Exchange(Vector3 delta, float dT)
        {
            reservoir = reservoir * (1.0f - stability);
            reservoir += delta;
            Vector3 leak = reservoir * (0.0f + leakRate * dT);
            reservoir -= leak;
            return leak;
        }
    }

    public class QuaternionReservoir: AbstractReservoir<Quaternion>
    {
        public QuaternionReservoir(float _stability)
        {
            // Quaternion.Lerp fails when alpha is ~ 1E-6. Just make it 0.01 min
            stability = Mathf.Max(_stability, 0.01f);
        }

        public override Quaternion Exchange(Quaternion delta, float dT)
        {
            reservoir = Quaternion.Lerp(reservoir, delta, dT);
            return (reservoir);
        }
    }
}
