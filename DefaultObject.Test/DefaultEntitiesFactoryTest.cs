using System.Collections.Generic;
using System.Linq;
using DevRating.DefaultObject.Fake;
using DevRating.Domain;
using Xunit;

namespace DevRating.DefaultObject.Test
{
    public sealed class DefaultEntityFactoryTest
    {
        [Fact]
        public void InsertsNewAuthor()
        {
            var authors = new List<Author>();

            new DefaultEntityFactory(
                    new FakeEntities(
                        new List<Work>(),
                        authors,
                        new List<Rating>()
                    ),
                    new FakeFormula()
                )
                .InsertedWork(
                    "repository",
                    "start",
                    "end",
                    "new author",
                    0u,
                    new DefaultEnvelope()
                );

            Assert.Single(authors);
        }

        [Fact]
        public void DoesntInsertAuthorIfExists()
        {
            var authors = new List<Author> {new FakeAuthor("existing author")};

            new DefaultEntityFactory(
                    new FakeEntities(
                        new List<Work>(),
                        authors,
                        new List<Rating>()
                    ),
                    new FakeFormula()
                )
                .InsertedWork(
                    "repository",
                    "start",
                    "end",
                    "existing author",
                    0u,
                    new DefaultEnvelope()
                );

            Assert.Single(authors);
        }

        [Fact]
        public void InsertsNewWorkWithoutUsedRating()
        {
            var works = new List<Work>();

            new DefaultEntityFactory(
                    new FakeEntities(
                        works,
                        new List<Author>(),
                        new List<Rating>()),
                    new FakeFormula()
                )
                .InsertedWork(
                    "repository",
                    "start",
                    "end",
                    "other author",
                    0u,
                    new DefaultEnvelope()
                );

            Assert.False(works.Single().UsedRating().Id().Filled());
        }

        [Fact]
        public void InsertsNewWorkWithUsedRating()
        {
            var author = new FakeAuthor("the author");
            var work = new FakeWork(10u, author);
            var rating = new FakeRating(1500d, work, author);
            var works = new List<Work> {work};

            new DefaultEntityFactory(
                    new FakeEntities(
                        works,
                        new List<Author> {author},
                        new List<Rating> {rating}
                    ),
                    new FakeFormula()
                )
                .InsertedWork(
                    "repository",
                    "start",
                    "end",
                    author.Email(),
                    0u,
                    new DefaultEnvelope()
                );

            Assert.Equal(rating, works.Last().UsedRating());
        }

        [Fact]
        public void InsertsNewAuthorNewRatingWhenDeleteOneNewVictim()
        {
            var ratings = new List<Rating>();
            var author = new FakeAuthor("author");
            var formula = new FakeFormula(10, 1);
            var deletion = new DefaultDeletion("single victim", 2);
            var newWork = new FakeWork(0u, author);

            new DefaultEntityFactory(
                    new FakeEntities(
                        new List<Work> {newWork},
                        new List<Author>(),
                        ratings
                    ),
                    formula
                )
                .InsertRatings(author.Email(), new[] {deletion}, newWork.Id());

            bool RatingOfAuthor(Rating r)
            {
                return r.Author().Email().Equals(author.Email());
            }

            Assert.Equal(
                formula.WinnerNewRating(
                    formula.DefaultRating(),
                    new[]
                    {
                        new DefaultMatch(formula.DefaultRating(), deletion.Count())
                    }
                ),
                ratings.Single(RatingOfAuthor).Value()
            );
        }

        [Fact]
        public void DoesntInsertsNewAuthorNewRatingWhenHeDeletedHimself()
        {
            var ratings = new List<Rating>();
            var author = new FakeAuthor("author");
            var formula = new FakeFormula(10, 1);
            var deletion = new DefaultDeletion("AUTHOR", 2);

            new DefaultEntityFactory(
                    new FakeEntities(
                        new List<Work>(),
                        new List<Author>(),
                        ratings
                    ),
                    formula
                )
                .InsertRatings(author.Email(), new[] {deletion}, new FakeWork(0u, author).Id());

            Assert.Empty(ratings);
        }

        [Fact]
        public void InsertsNewVictimNewRating()
        {
            var ratings = new List<Rating>();
            var author = new FakeAuthor("author");
            var formula = new FakeFormula(10, 1);
            var victim = "single victim";
            var deletion = new DefaultDeletion(victim, 2);
            var newWork = new FakeWork(0u, author);

            new DefaultEntityFactory(
                    new FakeEntities(
                        new List<Work> {newWork},
                        new List<Author>(),
                        ratings
                    ),
                    formula
                )
                .InsertRatings(author.Email(), new[] {deletion}, newWork.Id());

            bool RatingOfVictim(Rating r)
            {
                return r.Author().Email().Equals(victim);
            }

            Assert.Equal(
                formula.LoserNewRating(
                    formula.DefaultRating(),
                    new DefaultMatch(formula.DefaultRating(), deletion.Count())
                ),
                ratings.Single(RatingOfVictim).Value()
            );
        }

        [Fact]
        public void InsertsNewAuthorNewRatingWhenDeleteManyNewVictims()
        {
            var author = new FakeAuthor("author");
            var victim1 = "first victim";
            var victim2 = "second victim";
            var ratings = new List<Rating>();
            var formula = new FakeFormula(10, 1);
            var deletion1 = new DefaultDeletion(victim1, 2);
            var deletion2 = new DefaultDeletion(victim2, 3);
            var work = new FakeWork(0u, author);

            new DefaultEntityFactory(
                    new FakeEntities(
                        new List<Work> {work},
                        new List<Author>(),
                        ratings
                    ),
                    formula
                )
                .InsertRatings(author.Email(), new[] {deletion1, deletion2}, work.Id());

            bool RatingOfAuthor(Rating r)
            {
                return r.Author().Email().Equals(author.Email());
            }

            Assert.Equal(
                formula.WinnerNewRating(
                    formula.DefaultRating(),
                    new[]
                    {
                        new DefaultMatch(formula.DefaultRating(), deletion1.Count()),
                        new DefaultMatch(formula.DefaultRating(), deletion2.Count())
                    }
                ),
                ratings.Single(RatingOfAuthor).Value()
            );
        }

        [Fact]
        public void InsertsNewAuthorNewRatingWhenDeleteOneOldVictim()
        {
            var author = new FakeAuthor("new author");
            var victim = new FakeAuthor("old single victim");
            var work = new FakeWork(3, victim);
            var newWork = new FakeWork(0u, author);
            var ratings = new List<Rating>
            {
                new FakeRating(50, work, victim)
            };
            var formula = new FakeFormula(10, 1);
            var deletion = new DefaultDeletion(victim.Email(), 2);


            new DefaultEntityFactory(
                    new FakeEntities(
                        new List<Work> {work, newWork},
                        new List<Author> {victim},
                        ratings
                    ),
                    formula
                )
                .InsertRatings(author.Email(), new[] {deletion}, newWork.Id());

            bool RatingOfAuthor(Rating r)
            {
                return r.Author().Email().Equals(author.Email());
            }

            Assert.Equal(
                formula.WinnerNewRating(
                    formula.DefaultRating(),
                    new[]
                    {
                        new DefaultMatch(formula.DefaultRating(), deletion.Count())
                    }
                ),
                ratings.Single(RatingOfAuthor).Value()
            );
        }

        [Fact]
        public void InsertsOldVictimNewRating()
        {
            var author = new FakeAuthor("new author");
            var victim = new FakeAuthor("old single victim");
            var work = new FakeWork(3, victim);
            var newWork = new FakeWork(0u, author);
            var ratings = new List<Rating>
                {new FakeRating(50, work, victim)};
            var formula = new FakeFormula(10, 1);
            var deletion = new DefaultDeletion(victim.Email(), 2);

            new DefaultEntityFactory(
                    new FakeEntities(
                        new List<Work> {work, newWork},
                        new List<Author>(),
                        ratings
                    ),
                    formula
                )
                .InsertRatings(author.Email(), new[] {deletion}, newWork.Id());

            bool RatingOfVictim(Rating r)
            {
                return r.Author().Email().Equals(victim.Email());
            }

            Assert.Equal(
                formula.LoserNewRating(
                    formula.DefaultRating(),
                    new DefaultMatch(formula.DefaultRating(), deletion.Count())
                ),
                ratings.Last(RatingOfVictim).Value()
            );
        }

        [Fact]
        public void InsertsNewAuthorNewRatingWhenDeleteManyOldVictims()
        {
            var author = new FakeAuthor("new author");
            var victim1 = new FakeAuthor("old first victim");
            var victim2 = new FakeAuthor("old second victim");
            var work1 = new FakeWork(3, victim1);
            var work2 = new FakeWork(4, victim2);
            var newWork = new FakeWork(0u, author);
            var ratings = new List<Rating>
            {
                new FakeRating(50, work1, victim1),
                new FakeRating(40, work2, victim2)
            };
            var formula = new FakeFormula(10, 1);
            var deletion1 = new DefaultDeletion(victim1.Email(), 2);
            var deletion2 = new DefaultDeletion(victim2.Email(), 3);


            new DefaultEntityFactory(
                    new FakeEntities(
                        new List<Work> {work1, work2, newWork},
                        new List<Author>(),
                        ratings
                    ),
                    formula
                )
                .InsertRatings(author.Email(), new[] {deletion1, deletion2}, newWork.Id());

            bool RatingOfAuthor(Rating r)
            {
                return r.Author().Email().Equals(author.Email());
            }

            Assert.Equal(
                formula.WinnerNewRating(
                    formula.DefaultRating(),
                    new[]
                    {
                        new DefaultMatch(formula.DefaultRating(), deletion1.Count()),
                        new DefaultMatch(formula.DefaultRating(), deletion2.Count())
                    }
                ),
                ratings.Single(RatingOfAuthor).Value()
            );
        }

        [Fact]
        public void InsertsOldAuthorNewRatingWhenDeleteOneNewVictim()
        {
            var author = new FakeAuthor("old author");
            var victim = "single new victim";
            var work = new FakeWork(10u, author);
            var rating = new FakeRating(1500d, work, author);
            var ratings = new List<Rating> {rating};
            var formula = new FakeFormula(10, 1);
            var deletion = new DefaultDeletion(victim, 2);
            var newWork = new FakeWork(0u, author);

            new DefaultEntityFactory(
                    new FakeEntities(
                        new List<Work> {work, newWork},
                        new List<Author> {author},
                        ratings
                    ),
                    formula
                )
                .InsertRatings(author.Email(), new[] {deletion}, newWork.Id());

            bool RatingOfAuthor(Rating r)
            {
                return r.Author().Email().Equals(author.Email());
            }

            Assert.Equal(
                formula.WinnerNewRating(
                    rating.Value(),
                    new[]
                    {
                        new DefaultMatch(formula.DefaultRating(), deletion.Count())
                    }
                ),
                ratings.Last(RatingOfAuthor).Value()
            );
        }
    }
}