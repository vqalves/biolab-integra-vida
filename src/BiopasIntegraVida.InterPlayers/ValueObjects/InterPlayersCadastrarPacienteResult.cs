﻿namespace BiopasIntegraVida.InterPlayers.ValueObjects
{
    public class InterPlayersCadastrarPacienteResult
    {
        public long? consumerInternalId { get; set; }

        public static bool JaPossuiCadastro(InterPlayersError? error) => error?.Deserialized?.data?.error == "LR10";
    }
}
