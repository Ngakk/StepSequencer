namespace StepSequencer
{
    public class StepEmpty : Step
    {
        public void Finish()
        {
            Complete();   
        }
        
        public void GoBack()
        {
            Undo();   
        }
    }
}
