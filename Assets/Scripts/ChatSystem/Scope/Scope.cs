using UnityEngine;

namespace NetickChatSystem
{
    public class Scope
    {
        public uint scope { get; private set; }
        public string name { get; private set; }

        public bool editable { get; private set; }

        // When checking with extended scope.
        // strict will only match the full extended scope
        // loose will match every part of the extended scope.
        public enum CheckPolicy
        {
            strict,
            loose
        }

        public CheckPolicy checkPolicy { get; private set; }

        public enum ForeignReceivePolicy
        {
            forbidden,
            authorized,
        }

        public ForeignReceivePolicy foreignReceivePolicy { get; private set; }

        private bool isRegistred = true;

        public static Scope Everyone { get => new Scope(0, "Everyone", false, ForeignReceivePolicy.authorized); }

        public Scope(uint scope, string name, bool editable = true, ForeignReceivePolicy foreignReceivePolicy = ForeignReceivePolicy.forbidden)
        {
            this.editable = editable;
            this.scope = scope;
            this.name = name;
            this.checkPolicy = checkPolicy;
            this.foreignReceivePolicy = foreignReceivePolicy;
        }

        // this one is called when a new player joins the game
        public Scope(uint scope, string name, bool editable = true)
        {
            this.editable = editable;
            this.scope = scope;
            this.name = name;
            this.checkPolicy = checkPolicy;
            this.foreignReceivePolicy = foreignReceivePolicy;
            this.isRegistred = false;
        }

        public Scope(string name, Scope[] scopes, bool editable = true, CheckPolicy checkPolicy = CheckPolicy.strict, ForeignReceivePolicy foreignReceivePolicy = ForeignReceivePolicy.forbidden)
        {
            this.editable = editable;
            this.name = name;
            this.foreignReceivePolicy = foreignReceivePolicy;
            this.checkPolicy = checkPolicy;
            uint newScope = scopes[0].scope;
            for (int i = 1; i < scopes.Length; i++)
            {
                newScope |= scopes[i].scope;
            }
            this.scope = newScope;
        }

        public Scope(Scope from)
        {
            this.editable = from.editable;
            this.scope = from.scope;
            this.name = from.name;
            this.checkPolicy = from.checkPolicy;
            this.foreignReceivePolicy = from.foreignReceivePolicy;
        }

        public void AddScope(Scope add)
        {
            if (!editable)
            {
                Debug.LogError("This scope cannot be edited");
                return;
            }
            this.scope |= add.scope;
        }

        public void RemoveScope(Scope remove)
        {
            if (!editable)
            {
                Debug.LogError("This scope cannot be edited");
                return;
            }
            this.scope = ((((~remove.scope) | this.scope) & remove.scope) ^ scope);
        }

        public void Clear()
        {
            if (!editable)
            {
                Debug.LogError("This scope cannot be edited");
                return;
            }
            this.scope = 0;
        }


        ///<summary>
        /// You will only be able to change a scope name if the scope is marked as editable and wasn't registred. The only scope you should change the name of, is a NetworkConnection scope (scope of a player).
        ///</summary>
        public void ChangeName(string name)
        {
            if (!editable && !isRegistred)
            {
                Debug.LogError("This scope cannot be edited");
                return;
            }
            this.name = name;
        }

        public bool CheckAgainst(Scope against)
        {
            switch (against.checkPolicy)
            {
                case CheckPolicy.strict:
                    return (this.scope & against.scope) == against.scope;
                case CheckPolicy.loose:
                    return (this.scope & against.scope) > 0;
                default:
                    return false;
            }
        }
    }
}