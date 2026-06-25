namespace PeluqueriaCanina.Models
{
    public class Cliente : Persona
    {
        public int NroCliente { get; set; }
        public List<Mascota> Mascotas { get; set; } = new List<Mascota>();

    }
}
