using System;
using Shoko.Server.Filters.Interfaces;

namespace Shoko.Server.Filters.Files;

public class HasResolutionExpression : FilterExpression<bool>, IWithStringParameter
{
    public HasResolutionExpression(string parameter)
    {
        Parameter = parameter;
    }
    public HasResolutionExpression() { }

    public string Parameter { get; set; }
    public override bool TimeDependent => false;
    public override bool UserDependent => false;
    public override string HelpDescription => "This condition passes if any of the files have the specified video resolution";
    public override string[] HelpPossibleParameters => new[]
    {
        "2160p","1080p","720p","480p","UWHD","UWQHD","1440p","576p","360p","240p"
    };

    public override bool Evaluate(IFilterable filterable, IFilterableUserInfo userInfo)
    {
        return filterable.Resolutions.Contains(Parameter);
    }

    protected bool Equals(HasResolutionExpression other)
    {
        return base.Equals(other) && Parameter == other.Parameter;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj))
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj.GetType() != this.GetType())
        {
            return false;
        }

        return Equals((HasResolutionExpression)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), Parameter);
    }

    public static bool operator ==(HasResolutionExpression left, HasResolutionExpression right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(HasResolutionExpression left, HasResolutionExpression right)
    {
        return !Equals(left, right);
    }
}
