using CVMatchPro; // pour accéder à MLModel.ModelInput / ModelOutput

namespace CVMatchPro.Services
{
    public class MatchingService
    {
        public float EvaluerPertinence(string competences, string experience, string formation)
        {
            var input = new MLModel.ModelInput
            {
                Competences = competences ?? string.Empty,
                Experience = experience ?? string.Empty,
                Formation = formation ?? string.Empty
            };

            var result = MLModel.Predict(input); // ✅ renvoie un ModelOutput
            var score = result.PredictedScore;

            // ✅ Sécuriser le score
            if (float.IsNaN(score) || float.IsInfinity(score))
            {
                score = 0f;
            }

            // ✅ Normaliser entre 0 et 1
            if (score < 0f) score = 0f;
            if (score > 1f) score = 1f;

            return score;
        }
    }
}
