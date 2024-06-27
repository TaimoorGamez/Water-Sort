namespace Core.GamePlay
{
    public class WaterShaderColorAssigner : ColorAssigner
    {
        int _colorCounter = 1;

        protected override void Start()
        {
            base.Start();
            _propBlock.SetColor("_Color" + _colorCounter.ToString(), SkinColor);
            _colorCounter++;
            _propBlock.SetColor("_Color" + _colorCounter.ToString(), SkinColor);
            _colorCounter++;
            _propBlock.SetColor("_Color" + _colorCounter.ToString(), SkinColor);
            _colorCounter++;
            _propBlock.SetColor("_Color" + _colorCounter.ToString(), SkinColor);
            MySkin.SetPropertyBlock(_propBlock);
        }
    }
}
