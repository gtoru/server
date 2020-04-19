using System;
using System.Collections.Generic;
using server.core.Domain.Error;
using server.core.Domain.Misc;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local
// ReSharper disable UnusedAutoPropertyAccessor.Local

namespace server.core.Domain.Tasks
{
    public class TestSession
    {
        private static readonly TimeSpan TimeToComplete = TimeSpan.FromMinutes(30);

        private TestSession()
        {
        }

        private TestSession(
            DateTime started,
            bool isFinished,
            int result,
            int possibleResult,
            Quiz quiz,
            User user,
            List<string> answers,
            ITimeProvider timeProvider)
        {
            TimeProvider = timeProvider;
            Started = started;
            IsFinished = isFinished;
            Result = result;
            PossibleResult = possibleResult;
            Quiz = quiz;
            User = user;
            UserId = user.UserId;
            Answers = answers;
        }

        public ITimeProvider TimeProvider { get; private set; }
        public Guid SessionId { get; private set; }
        public DateTime Started { get; private set; }
        public bool IsFinished { get; private set; }
        public int Result { get; private set; }
        public int PossibleResult { get; private set; }
        public Quiz Quiz { get; private set; }
        public User User { get; private set; }
        public Guid UserId { get; private set; }
        public List<string> Answers { get; private set; }

        public static TestSession CreateNew(User user, Quiz quiz, ITimeProvider timeProvider = null)
        {
            timeProvider ??= new UtcTimeProvider();

            var possibleResult = quiz.Tasks.Count;

            var answers = new List<string>(possibleResult);

            for (var i = 0; i < possibleResult; ++i)
                answers.Add("#");

            var started = timeProvider.GetCurrent();

            return new TestSession(
                started,
                false,
                0,
                possibleResult,
                quiz,
                user,
                answers,
                timeProvider);
        }

        public void Answer(int taskNumber, string guess)
        {
            if (taskNumber < 0 || taskNumber >= Answers.Count)
                throw new TaskNumberOutOfRangeException();

            if (IsFinished || Expired())
            {
                IsFinished = true;
                throw new TestSessionAlreadyFinishedException();
            }

            Answers[taskNumber] = guess;
        }

        public void Finish()
        {
            if (IsFinished)
                return;

            IsFinished = true;

            for (var i = 0; i < Answers.Count; ++i)
                if (Quiz.Tasks[i].Task.CheckAnswer(Answers[i]))
                    Result += 1;
        }

        public int GetResult()
        {
            if (IsFinished || Expired())
            {
                Finish();
                return Result;
            }

            throw new SessionNotFinishedException();
        }

        public bool Expired()
        {
            return Started + TimeToComplete < TimeProvider.GetCurrent();
        }
    }
}
