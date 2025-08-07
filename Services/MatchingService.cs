namespace CVMatchPro.Services
{
    public class MatchingService
    {
        public float EvaluerPertinence(string competences, string experience, string formation)
        {
            var input = new MLModel.ModelInput
            {
                Competences = competences,
                Experience = experience,
                Formation = formation
            };

            MLModel.ModelOutput result = MLModel.Predict(input);

            return result.Score; // score de matching (prédit par le modèle ML)
        }
    }
}
