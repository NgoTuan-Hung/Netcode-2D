
public class TransitionRule
{
    public TransitionRule(bool onAirOutcome, bool onGroundOutcome) => (this.onAirOutcome, this.onGroundOutcome) = (onAirOutcome, onGroundOutcome);
    private bool onAirOutcome;
    private bool onGroundOutcome;

    public bool OnAirOutcome { get => onAirOutcome; set => onAirOutcome = value; }
    public bool OnGroundOutcome { get => onGroundOutcome; set => onGroundOutcome = value; }
}