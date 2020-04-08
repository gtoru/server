using System;
using System.Collections.Generic;
using server.core.Domain.Error;
using server.core.Domain.Misc;

namespace server.core.Domain.Tasks
{
    public class TestSession
    {
        private static readonly TimeSpan TimeToComplete = TimeSpan.FromMinutes(30);

        public TestSession()
        {
        }

        public TestSession(
            Guid sessionId,
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
            SessionId = sessionId;
            Started = started;
            IsFinished = isFinished;
            Result = result;
            PossibleResult = possibleResult;
            Quiz = quiz;
            User = user;
            Answers = answers;
        }

        public ITimeProvider TimeProvider { get; set; }
        public Guid SessionId { get; set; }
        public DateTime Started { get; set; }
        public bool IsFinished { get; set; }
        public int Result { get; set; }
        public int PossibleResult { get; set; }
        public Quiz Quiz { get; set; }
        public User User { get; set; }
        public List<string> Answers { get; set; }

        public static TestSession StartNew(User user, Quiz quiz, ITimeProvider timeProvider = null)
        {
            var id = Guid.NewGuid();

            timeProvider ??= new UtcTimeProvider();

            quiz.Lock();

            var possibleResult = quiz.Tasks.Count;

            var answers = new List<string>(possibleResult);

            for (var i = 0; i < possibleResult; ++i)
                answers.Add("#");

            var started = timeProvider.GetCurrent();

            return new TestSession(
                id,
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

            if (IsFinished || Started + TimeToComplete < TimeProvider.GetCurrent())
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
    }
}
