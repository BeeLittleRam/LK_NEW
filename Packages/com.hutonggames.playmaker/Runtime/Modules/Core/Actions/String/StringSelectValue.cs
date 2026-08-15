using JetBrains.Annotations;

namespace HutongGames.PlayMaker.Actions
{
    [System.Serializable]
    [PublicAPI]
    [ActionCategory(Category.String)]
    [ActionDescription("Set a String variable to the True Value or False Value based on a Bool.")]
    public class StringSelectValue : BaseSelectValue<StringVar, StringRef>
    {
        public override void Reset()
        {
            base.Reset();
            SetDefaults("True", "False");
        }
    }
}
