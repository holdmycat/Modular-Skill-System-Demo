//------------------------------------------------------------
// File: InputBlocker.cs
// Purpose: Centralized input blocking utility (manager layer, scene/character bootstrap).
//------------------------------------------------------------
using System;
using Ebonor.DataCtrl;
using Ebonor.GamePlay;

namespace Ebonor.Manager
{
    /// <summary>
    /// Use to block input groups globally (e.g., during scene loading). PlayerInputRouter queries via InputBlockRegistry.
    /// </summary>
    public static class InputBlocker
    {
        private sealed class BlockHandle : IDisposable
        {
            private readonly eInputControlFlag _flags;
            private bool _disposed;

            public BlockHandle(eInputControlFlag flags)
            {
                _flags = flags;
                Block(flags);
            }

            public void Dispose()
            {
                if (_disposed) return;
                _disposed = true;
                Unblock(_flags);
            }
        }

        private static int _movementBlocks;
        private static int _skillBlocks;
        private static int _uiBlocks;

        static InputBlocker()
        {
            // Expose this blocker to gameplay layer via registry to avoid assembly dependency cycles.
            InputBlockRegistry.Source = new BlockSourceProxy();
        }

        private class BlockSourceProxy : IInputBlockSource
        {
            public bool IsBlocked(eInputControlFlag flag) => InputBlocker.IsBlocked(flag);
            public eInputControlFlag BlockedFlags => InputBlocker.BlockedFlags;
        }

        /// <summary>Current blocked groups mask.</summary>
        public static eInputControlFlag BlockedFlags
        {
            get
            {
                eInputControlFlag flags = eInputControlFlag.None;
                if (_movementBlocks > 0) flags |= eInputControlFlag.Movement;
                if (_skillBlocks > 0) flags |= eInputControlFlag.Skills;
                if (_uiBlocks > 0) flags |= eInputControlFlag.Ui;
                return flags;
            }
        }

        public static bool IsBlocked(eInputControlFlag flag) => (BlockedFlags & flag) != 0;

        /// <summary>Acquire a blocking handle for specified groups; dispose to release.</summary>
        public static IDisposable Acquire(eInputControlFlag flags)
        {
            return new BlockHandle(flags);
        }

        /// <summary>Set block counts for the given flags.</summary>
        public static void Block(eInputControlFlag flags)
        {
            if ((flags & eInputControlFlag.Movement) != 0) _movementBlocks++;
            if ((flags & eInputControlFlag.Skills) != 0) _skillBlocks++;
            if ((flags & eInputControlFlag.Ui) != 0) _uiBlocks++;
        }

        /// <summary>Release block counts for the given flags.</summary>
        public static void Unblock(eInputControlFlag flags)
        {
            if ((flags & eInputControlFlag.Movement) != 0 && _movementBlocks > 0) _movementBlocks--;
            if ((flags & eInputControlFlag.Skills) != 0 && _skillBlocks > 0) _skillBlocks--;
            if ((flags & eInputControlFlag.Ui) != 0 && _uiBlocks > 0) _uiBlocks--;
        }

        /// <summary>Convenience: block all groups.</summary>
        public static IDisposable AcquireAll() => Acquire(eInputControlFlag.All);
    }
}
