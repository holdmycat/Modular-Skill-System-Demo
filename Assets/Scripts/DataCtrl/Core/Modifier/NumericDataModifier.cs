namespace Ebonor.DataCtrl
{
    public class NumericDataModifier : ADataModifier
    {
        private float _value;
        private ModifierType _type;

        public override ModifierType ModifierType => _type;

        public void Initialize(ModifierType type, float value)
        {
            _type = type;
            _value = value;
        }
        
        // Update method to allow changing value in-place
        public void UpdateValue(float newValue)
        {
            _value = newValue;
        }

        public override float GetModifierValue()
        {
            return _value;
        }

        public override void Clear()
        {
            _value = 0;
            _type = ModifierType.Constant;
        }

        public static NumericDataModifier Create(ModifierType type, float value)
        {
            var mod = ReferencePool.Acquire<NumericDataModifier>();
            mod.Initialize(type, value);
            return mod;
        }
    }
}
