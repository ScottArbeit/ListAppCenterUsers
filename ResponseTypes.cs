using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace ListAppCenterUsers
{
    public class TeamResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("display_name")]
        public string DisplayName { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }
    }

    public class UserResponse
    {
        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("display_name")]
        public string DisplayName { get; set; }

        [JsonPropertyName("role")]
        public string Role { get; set; }
    }

    public class TeamUserResponse : UserResponse
    {
    }

    public class OrganizationUserResponse : UserResponse
    {
        [JsonPropertyName("joined_at")]
        public string JoinedAt { get; set; }
    }

    public class DistributionGroupResponse
    {
        [JsonPropertyName("id")]
        public System.Guid Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("display_name")]
        public string DisplayName { get; set; }

        [JsonPropertyName("origin")]
        public string Origin { get; set; }

        [JsonPropertyName("is_public")]
        public bool IsPublic { get; set; }
    }

    public class DistributionGroupMembersResponse
    {
        [JsonPropertyName("id")]
        public System.Guid? Id { get; set; }

        [JsonPropertyName("avatar_url")]
        public string AvatarUrl { get; set; }

        [JsonPropertyName("can_change_password")]
        public bool? CanChangePassword { get; set; }

        [JsonPropertyName("display_name")]
        public string DisplayName { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("invite_pending")]
        public bool? InvitePending { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

    }

    public class Owner
    {
        [JsonPropertyName("id")]
        public System.Guid Id { get; set; }

        [JsonPropertyName("avatar_url")]
        public string AvatarUrl { get; set; }

        [JsonPropertyName("display_name")]
        public string DisplayName { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }
    }

    public class AzureSubscriptionResponse
    {
        [JsonPropertyName("subscription_id")]
        public System.Guid SubscriptionId { get; set; }

        [JsonPropertyName("tenant_id")]
        public System.Guid TenantId { get; set; }

        [JsonPropertyName("subscription_name")]
        public string SubscriptionName { get; set; }

        [JsonPropertyName("is_billing")]
        public bool? IsBilling { get; set; }

        [JsonPropertyName("is_billable")]
        public bool? IsBillable { get; set; }

        [JsonPropertyName("is_microsoft_internal")]
        public bool? IsMicrosoftInternal { get; set; }
    }

    public class AppResponse
    {
        [JsonPropertyName("id")]
        public System.Guid Id { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("display_name")]
        public string DisplayName { get; set; }

        [JsonPropertyName("release_type")]
        public string ReleaseType { get; set; }

        [JsonPropertyName("icon_url")]
        public string IconUrl { get; set; }

        [JsonPropertyName("icon_source")]
        public string IconSource { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("os")]
        public string Os { get; set; }

        [JsonPropertyName("owner")]
        public Owner Owner { get; set; }

        [JsonPropertyName("app_secret")]
        public string AppSecret { get; set; }

        [JsonPropertyName("azure_subscription")]
        public AzureSubscriptionResponse AzureSubscription { get; set; }

        [JsonPropertyName("platform")]
        public string Platform { get; set; }

        [JsonPropertyName("origin")]
        public string Origin { get; set; }

        [JsonPropertyName("created_at")]
        public string CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public string UpdatedAt { get; set; }

        [JsonPropertyName("member_permissions")]
        public IList<string> MemberPermissions { get; set; }

    }
}
