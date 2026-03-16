#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Voltline.Data;

namespace Voltline.Editor
{
    public sealed class PresentationRefreshApprovalAuditResult
    {
        private readonly List<string> errors = new();
        private readonly List<string> warnings = new();
        private readonly List<string> notes = new();

        public IReadOnlyList<string> Errors => errors;
        public IReadOnlyList<string> Warnings => warnings;
        public IReadOnlyList<string> Notes => notes;
        public bool HasErrors => errors.Count > 0;

        public void AddError(string message) => errors.Add(message);
        public void AddWarning(string message) => warnings.Add(message);
        public void AddNote(string message) => notes.Add(message);
    }

    public static class PresentationRefreshApprovalAudit
    {
        [MenuItem("Tools/Voltline/Run Presentation Refresh Approval Audit")]
        private static void RunFromMenu()
        {
            PresentationRefreshApprovalAuditResult result = Validate();

            foreach (string note in result.Notes)
            {
                Debug.Log(note);
            }

            foreach (string warning in result.Warnings)
            {
                Debug.LogWarning(warning);
            }

            if (result.HasErrors)
            {
                foreach (string error in result.Errors)
                {
                    Debug.LogError(error);
                }

                return;
            }

            Debug.Log("Voltline presentation refresh approval audit passed with no blocking errors.");
        }

        public static PresentationRefreshApprovalAuditResult Validate()
        {
            PresentationRefreshApprovalAuditResult result = new();
            ReleaseAuditResult releaseAudit = ReleaseReadinessAudit.Validate();
            PresentationReadinessAuditResult readinessAudit = PresentationReadinessAudit.Validate();
            PresentationRolloutPlanConfig rolloutPlan = AssetDatabase.LoadAssetAtPath<PresentationRolloutPlanConfig>(ProjectConfigAssetPaths.PresentationRolloutPlan);

            for (int i = 0; i < releaseAudit.Errors.Count; i++)
            {
                result.AddError(releaseAudit.Errors[i]);
            }

            for (int i = 0; i < readinessAudit.Errors.Count; i++)
            {
                result.AddError(readinessAudit.Errors[i]);
            }

            if (rolloutPlan == null)
            {
                result.AddError("Presentation refresh approval gate is missing PresentationRolloutPlanConfig.");
                return result;
            }

            AddManualGateNotes(result, rolloutPlan);

            for (int i = 0; i < releaseAudit.Warnings.Count; i++)
            {
                result.AddWarning(releaseAudit.Warnings[i]);
            }

            for (int i = 0; i < readinessAudit.Warnings.Count; i++)
            {
                result.AddWarning(readinessAudit.Warnings[i]);
            }

            return result;
        }

        private static void AddManualGateNotes(PresentationRefreshApprovalAuditResult result, PresentationRolloutPlanConfig rolloutPlan)
        {
            result.AddNote("Refresh approval gate requires config validation, release audit, presentation readiness audit, Edit Mode, Play Mode, manual readability review, manual collision review, device performance checks, save/theme persistence checks, build-size review, and no normal-flow console noise.");
            result.AddNote($"Controlled rollout is staged across {rolloutPlan.Slices.Count} slices and every slice blocks the next one until signoff.");
        }
    }
}
#endif
