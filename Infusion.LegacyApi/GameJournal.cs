﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Infusion.LegacyApi
{
    public sealed class GameJournal : IEnumerable<JournalEntry>
    {
        private readonly JournalSource source;
        private readonly Legacy legacyApi;
        private long journalEntryStartId;

        internal GameJournal(JournalSource source, Legacy legacyApi)
        {
            this.source = source;
            this.legacyApi = legacyApi;
            journalEntryStartId = source.CurrentJournalEntryId;
        }

        public IEnumerator<JournalEntry> GetEnumerator()
        {
            return source.Where(line => line.Id >= journalEntryStartId).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return source.Where(line => line.Id >= journalEntryStartId).GetEnumerator();
        }

        public bool Contains(params string[] words)
        {
            return source.Where(line => line.Id >= journalEntryStartId)
                .Any(line => words.Any(w => line.Message.IndexOf(w, StringComparison.OrdinalIgnoreCase) >= 0));
        }

        public void Delete()
        {
            journalEntryStartId = source.CurrentJournalEntryId;
        }

        public void WaitAny(params string[] words)
        {
            new JournalAwaiter(() => legacyApi.CancellationToken, source, this)
                .When(words, () => { })
                .WaitAny();
        }

        public JournalAwaiter When(string awaitedWord1, Action whenAction)
        {
            return new JournalAwaiter(() => legacyApi.CancellationToken, source, this)
                .When(awaitedWord1, whenAction);
        }

        public JournalAwaiter When(string awaitedWord1, string awaitedWord2, Action whenAction)
        {
            return new JournalAwaiter(() => legacyApi.CancellationToken, source, this)
                .When(awaitedWord1, awaitedWord2, whenAction);
        }

        public JournalAwaiter When(string awaitedWord1, string awaitedWord2, string awaitedWord3, Action whenAction)
        {
            return new JournalAwaiter(() => legacyApi.CancellationToken, source, this)
                .When(awaitedWord1, awaitedWord2, awaitedWord3, whenAction);
        }

        public JournalAwaiter When(string awaitedWord1, string awaitedWord2, string awaitedWord3, string awaitedWord4,
            Action whenAction)
        {
            return new JournalAwaiter(() => legacyApi.CancellationToken, source, this)
                .When(awaitedWord1, awaitedWord2, awaitedWord3, awaitedWord4, whenAction);
        }

        public JournalAwaiter When(string awaitedWord1, string awaitedWord2, string awaitedWord3, string awaitedWord4,
            string awaitedWord5, Action whenAction)
        {
            return new JournalAwaiter(() => legacyApi.CancellationToken, source, this)
                .When(awaitedWord1, awaitedWord2, awaitedWord3, awaitedWord4, awaitedWord5, whenAction);
        }

        public JournalAwaiter When(string[] awaitedWords, Action whenAction)
        {
            return new JournalAwaiter(() => legacyApi.CancellationToken, source, this)
                .When(awaitedWords, whenAction);
        }

        public JournalAwaiter When(string[] awaitedWords, Action<JournalEntry> whenAction)
        {
            return new JournalAwaiter(() => legacyApi.CancellationToken, source, this)
                .When(awaitedWords, whenAction);
        }

        public JournalAwaiter When(string awaitedWord1, Action<JournalEntry> whenAction)
        {
            return new JournalAwaiter(() => legacyApi.CancellationToken, source, this)
                .When(awaitedWord1, whenAction);
        }

        public JournalAwaiter When(string awaitedWord1, string awaitedWord2, Action<JournalEntry> whenAction)
        {
            return new JournalAwaiter(() => legacyApi.CancellationToken, source, this)
                .When(awaitedWord1, awaitedWord2, whenAction);
        }

        public JournalAwaiter When(string awaitedWord1, string awaitedWord2, string awaitedWord3,
            Action<JournalEntry> whenAction)
        {
            return new JournalAwaiter(() => legacyApi.CancellationToken, source, this)
                .When(awaitedWord1, awaitedWord2, awaitedWord3, whenAction);
        }

        public JournalAwaiter When(string awaitedWord1, string awaitedWord2, string awaitedWord3, string awaitedWord4,
            Action<JournalEntry> whenAction)
        {
            return new JournalAwaiter(() => legacyApi.CancellationToken, source, this)
                .When(awaitedWord1, awaitedWord2, awaitedWord3, awaitedWord4, whenAction);
        }

        public JournalAwaiter When(string awaitedWord1, string awaitedWord2, string awaitedWord3, string awaitedWord4,
            string awaitedWord5, Action<JournalEntry> whenAction)
        {
            return new JournalAwaiter(() => legacyApi.CancellationToken, source, this)
                .When(awaitedWord1, awaitedWord2, awaitedWord3, awaitedWord4, awaitedWord5, whenAction);
        }

        private long lastWaitEntryId = 0;

        internal void NotifyWait()
        {
            lastWaitEntryId = source.CurrentJournalEntryId;
        }

        internal IEnumerable<JournalEntry> AfterLastAction() =>
            source.AfterLastAction().Where(line => line.Id >= lastWaitEntryId);
    }
}