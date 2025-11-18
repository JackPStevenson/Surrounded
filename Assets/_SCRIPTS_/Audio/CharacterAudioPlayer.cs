
public class CharacterAudioPlayer : IndividualAudioPlayer {
    public void AttemptFillAudioNames() {
        if (type == SoundType.Zombie) {
            clipIndexes = new int[3];
            DataZombie data = gameObject.GetComponent<ZombieCore>().Data;
            if (data != null) {
                clipNames = new string[3];
                clipNames[0] = data.hurtSound;
                clipNames[1] = data.deathSound;
                clipNames[2] = data.attackSound;
            }
        }
        else if (type == SoundType.PoorSoul) {

        }
    }

    public override void GetAudioIndexes() {
        AttemptFillAudioNames();
        base.GetAudioIndexes();
    }

    public void PlayHurtSound() => PlaySound(0);
    public void PlayDeathSound() => PlaySound(1);
    public void PlayAttackSound() => PlaySound(2);
}